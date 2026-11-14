using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingAttempts;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingAssessments.GetShadowingAttemptAssessments;

// ---- DTOs ----

public sealed record AssessmentDetailDto(
	Guid Id,
	Guid AssessmentId,
	Guid AttemptDetailId,
	decimal? AccuracyScore,
	string? AccuracyAssessmentResponse,
	decimal? PitchSimilarityScore,
	string? PitchSimilarityAssessmentResponse,
	string? Feedback);

public sealed record AssessmentWithDetailsDto(
	Guid Id,
	Guid AttemptId,
	ShadowingAssessmentType Type,
	decimal? OverallScore,
	string? OverallFeedback,
	DateTimeOffset CreatedAt,
	IReadOnlyList<AssessmentDetailDto> Details);

/// <summary>
/// Mirrors GET /api/shadowing-sessions/{sessionId}/attempts/{attemptId}
/// but returns attempt + its assessment(s).
/// Cancelled attempts will have an empty Assessments list.
/// </summary>
public sealed record AttemptWithAssessmentsDto(
	// --- same shape as AttemptViewDto ---
	AttemptSummaryDto Attempt,
	PaginatedList<AttemptDetailDto> Details,
	IReadOnlyList<MissingRoleLineDto> MissingLines,
	// --- assessment data ---
	IReadOnlyList<AssessmentWithDetailsDto> Assessments);

// ---- QUERY ----

public sealed record GetShadowingAttemptAssessmentsQuery(
	Guid SessionId,
	Guid AttemptId,
	int PageNumber,
	int PageSize,
	bool? IsSkipped,
	Guid? MemberId)         // null → caller (default: the member who calls the API)
	: IRequest<Result<AttemptWithAssessmentsDto>>;

// ---- VALIDATOR ----

internal sealed class GetShadowingAttemptAssessmentsValidator
	: AbstractValidator<GetShadowingAttemptAssessmentsQuery>
{
	public GetShadowingAttemptAssessmentsValidator()
	{
		RuleFor(x => x.SessionId).NotEmpty();
		RuleFor(x => x.AttemptId).NotEmpty();
		RuleFor(x => x.PageNumber).GreaterThan(0);
		RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
	}
}

// ---- HANDLER ----

internal sealed class GetShadowingAttemptAssessmentsHandler(
	IShadowingSessionRepository sessionRepository,
	IShadowingAttemptRepository attemptRepository,
	IShadowingRoleLineRepository roleLineRepository,
	IStorageService storage,
	ICurrentUserContext currentUser)
	: IRequestHandler<GetShadowingAttemptAssessmentsQuery, Result<AttemptWithAssessmentsDto>>
{
	public async Task<Result<AttemptWithAssessmentsDto>> Handle(
		GetShadowingAttemptAssessmentsQuery request,
		CancellationToken cancellationToken)
	{
		if (currentUser.UserId is null)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingSessionErrors.Unauthenticated);

		// --- Validate session membership ----------------------------------------
		var session = await sessionRepository.GetByIdAsync(
			ShadowingSessionId.From(request.SessionId), cancellationToken);
		if (session is null)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingSessionErrors.NotFound);

		var isCurator = AttemptMapper.IsCurator(currentUser);
		var callerIsMember = session.Members.Any(m => m.MemberId == currentUser.UserId);
		if (!isCurator && !callerIsMember)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingSessionErrors.NotMember);

		// --- Resolve the target member (default = caller) -----------------------
		//     When MemberId is provided the target must still be a session member.
		Guid resolvedMemberId = request.MemberId ?? currentUser.UserId!.Value;
		var targetMember = session.Members.FirstOrDefault(m => m.MemberId.Value == resolvedMemberId);
		if (targetMember is null)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingSessionErrors.NotMember);

		// --- Load the specific attempt by ID ------------------------------------
		var attempt = await attemptRepository.GetWithContextAsync(
			ShadowingAttemptId.From(request.AttemptId), cancellationToken);

		if (attempt is null || attempt.ShadowingSessionId.Value != request.SessionId)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingAttemptErrors.NotFound);

		// The attempt must belong to the resolved member
		if (attempt.ShadowingSessionMember.MemberId.Value != resolvedMemberId)
			return Result.Failure<AttemptWithAssessmentsDto>(ShadowingAttemptErrors.NotOwner);

		// --- Paged attempt details (mirror of GetShadowingAttemptQuery) ---------
		var (details, total) = await attemptRepository.GetDetailsPagedAsync(
			attempt.Id, request.PageNumber, request.PageSize, cancellationToken, request.IsSkipped);

		var detailDtos = new List<AttemptDetailDto>();
		foreach (var d in details)
		{
			var url = string.IsNullOrWhiteSpace(d.AudioKey)
				? null
				: await storage.CreatePresignedReadUrlAsync(d.AudioKey, cancellationToken);

			detailDtos.Add(new AttemptDetailDto(
				d.Id.Value, attempt.Id.Value, d.ShadowingRoleLineId.Value,
				d.AudioKey, url,
				d.DurationMs, d.IsSkipped, d.SubmittedAt));
		}

		// --- Missing lines for cancelled attempts --------------------------------
		var missing = new List<MissingRoleLineDto>();
		var roleId = attempt.ShadowingSessionMember.ShadowingRoleId;
		if (attempt.Status == ShadowingAttemptStatus.Cancelled && roleId is not null)
		{
			var (lines, _) = await roleLineRepository.GetPagedAsync(
				attempt.ShadowingSession.ShadowingMaterialId, null, roleId, 1, int.MaxValue, cancellationToken);
			var done = (await attemptRepository.GetDetailsAsync(attempt.Id, cancellationToken))
				.Select(d => d.ShadowingRoleLineId).ToHashSet();
			missing.AddRange(lines
				.Where(l => !done.Contains(l.Id))
				.Select(l => new MissingRoleLineDto(l.Id.Value, l.SortOrder, l.Text, l.StartTime, l.EndTime)));
		}

		// --- Assessments ---------------------------------------------------------
		//     Cancelled attempts have no assessments (by design).
		var assessmentDtos = new List<AssessmentWithDetailsDto>();
		if (attempt.Status != ShadowingAttemptStatus.Cancelled)
		{
			var assessments = await attemptRepository.GetAssessmentsWithDetailsAsync(
				attempt.Id, cancellationToken);

			foreach (var assessment in assessments)
			{
				var assessmentDetailDtos = assessment.Details
					.Select(ad => new AssessmentDetailDto(
						ad.Id.Value,
						ad.ShadowingAssessmentId.Value,
						ad.ShadowingAttemptDetailId.Value,
						ad.AccuracyScore,
						ad.AccuracyAssessmentResponse,
						ad.PitchSimilarityScore,
						ad.PitchSimilarityAssessmentResponse,
						ad.Feedback))
					.ToList();

				assessmentDtos.Add(new AssessmentWithDetailsDto(
					assessment.Id.Value,
					assessment.ShadowingAttemptId.Value,
					assessment.Type,
					assessment.OverallScore,
					assessment.OveralFeedback,
					assessment.CreatedAt,
					assessmentDetailDtos));
			}
		}

		return Result.Success(new AttemptWithAssessmentsDto(
			AttemptMapper.ToSummary(attempt),
			PaginatedList<AttemptDetailDto>.Create(detailDtos, total, request.PageNumber, request.PageSize),
			missing,
			assessmentDtos));
	}
}
