using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt.ShadowingAttemptDetail;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAssessments.ReevaluateShadowingAssessment;

public sealed record ReevaluatedDetailInput(
	Guid AttemptDetailId,
	decimal AccuracyScore,
	string? AccuracyAssessmentResponse,
	decimal? PitchSimilarityScore,
	string? PitchSimilarityAssessmentResponse,
	string Feedback);

public sealed record ReevaluateShadowingAssessmentCommand(
	Guid SessionId,
	Guid AttemptId,
	string OverallFeedback,
	IReadOnlyList<ReevaluatedDetailInput> Details) : IRequest<Result<Guid>>;

internal sealed class ReevaluateShadowingAssessmentValidator : AbstractValidator<ReevaluateShadowingAssessmentCommand>
{
	public ReevaluateShadowingAssessmentValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.AttemptId).NotEmpty();
		RuleFor(x => x.OverallFeedback).NotEmpty().WithMessage("OverallFeedback is required");
		RuleFor(x => x.Details).NotEmpty().WithMessage("At least one assessment detail is required");
		RuleFor(x => x.Details)
			.Must(d => d.Select(i => i.AttemptDetailId).Distinct().Count() == d.Count)
			.WithMessage("AttemptDetailId must not be duplicated");
		RuleForEach(x => x.Details).ChildRules(detail =>
		{
			detail.RuleFor(d => d.AttemptDetailId).NotEmpty();
			detail.RuleFor(d => d.AccuracyScore).InclusiveBetween(0m, 100m)
				.WithMessage("AccuracyScore must be between 0 and 100");
			detail.RuleFor(d => d.PitchSimilarityScore).InclusiveBetween(0m, 100m)
				.When(d => d.PitchSimilarityScore.HasValue)
				.WithMessage("PitchSimilarityScore must be between 0 and 100");
			detail.RuleFor(d => d.Feedback).NotEmpty().WithMessage("Feedback is required for each assessment detail");
		});
	}
}

internal sealed class ReevaluateShadowingAssessmentHandler(
	IShadowingAttemptRepository attemptRepository,
	ICurrentUserContext currentUser,
	IUnitOfWork unitOfWork)
	: IRequestHandler<ReevaluateShadowingAssessmentCommand, Result<Guid>>
{
	private const int MaxAssessmentsPerAttempt = 2;

	public async Task<Result<Guid>> Handle(ReevaluateShadowingAssessmentCommand request, CancellationToken cancellationToken)
	{
		return await unitOfWork.ExecuteAsync(async () =>
		{
			if (currentUser.UserId is null)
				return Result.Failure<Guid>(ShadowingSessionErrors.Unauthenticated);
			if (currentUser.Role != RoleCodes.LinguisticCurator)
				return Result.Failure<Guid>(ShadowingAttemptErrors.CuratorOnly);

			var attempt = await attemptRepository.GetWithContextAsync(ShadowingAttemptId.From(request.AttemptId), cancellationToken);
			if (attempt is null || attempt.ShadowingSessionId.Value != request.SessionId)
				return Result.Failure<Guid>(ShadowingAttemptErrors.NotFound);
			if (attempt.Status != ShadowingAttemptStatus.Assessed)
				return Result.Failure<Guid>(ShadowingAttemptErrors.NotAssessed);

			var assessments = await attemptRepository.GetAssessmentsWithDetailsAsync(attempt.Id, cancellationToken);
			if (assessments.Count == 0)
				return Result.Failure<Guid>(ShadowingAttemptErrors.NotAssessed);
			if (assessments.Count >= MaxAssessmentsPerAttempt)
				return Result.Failure<Guid>(ShadowingAttemptErrors.ReevaluationLimitReached);

			var attemptDetails = (await attemptRepository.GetDetailsAsync(attempt.Id, cancellationToken))
				.ToDictionary(d => d.Id.Value);
			foreach (var input in request.Details)
			{
				if (!attemptDetails.TryGetValue(input.AttemptDetailId, out var attemptDetail))
					return Result.Failure<Guid>(ShadowingAttemptErrors.DetailNotFound);
				if (attemptDetail.IsSkipped)
					return Result.Failure<Guid>(ShadowingAttemptErrors.InvalidAssessmentDetail);
			}

			var assessment = ShadowingAssessment.CreateCuratorReevaluation(
				attempt.Id, request.OverallFeedback.Trim(), DateTimeOffset.UtcNow);

			foreach (var input in request.Details)
			{
				assessment.AddDetail(
					ShadowingAttemptDetailId.From(input.AttemptDetailId),
					input.AccuracyScore,
					input.AccuracyAssessmentResponse,
					input.PitchSimilarityScore,
					input.PitchSimilarityAssessmentResponse,
					input.Feedback.Trim());
			}

			var updatedIds = request.Details.Select(d => d.AttemptDetailId).ToHashSet();
			var latest = assessments.OrderByDescending(a => a.CreatedAt).First();
			foreach (var old in latest.Details.Where(d => !updatedIds.Contains(d.ShadowingAttemptDetailId.Value)))
			{
				assessment.AddDetail(
					old.ShadowingAttemptDetailId,
					old.AccuracyScore ?? 0m,
					old.AccuracyAssessmentResponse,
					old.PitchSimilarityScore,
					old.PitchSimilarityAssessmentResponse,
					old.Feedback);
			}

			assessment.CalculateOverallScore();
			attemptRepository.AddAssessment(assessment);
			return Result.Success(assessment.Id.Value);
		}, cancellationToken);
	}
}
