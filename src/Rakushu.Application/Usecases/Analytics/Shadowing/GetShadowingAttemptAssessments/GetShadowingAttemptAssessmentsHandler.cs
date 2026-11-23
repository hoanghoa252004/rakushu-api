using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Shadowing.ShadowingAttempts;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;

namespace Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;

internal sealed class GetShadowingAttemptAssessmentsHandler
	: IRequestHandler<GetShadowingAttemptAssessmentsQuery, Result<ShadowingAttemptAssessmentsResponseDto>>
{
	private readonly ILearnerAnalyticsQueryService _analyticsQueryService;
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly IStorageService _storageService;
	private readonly ICurrentUserContext _currentUserContext;

	public GetShadowingAttemptAssessmentsHandler(
		ILearnerAnalyticsQueryService analyticsQueryService,
		IIssueDetailRepository issueDetailRepository,
		IStorageService storageService,
		ICurrentUserContext currentUserContext)
	{
		_analyticsQueryService = analyticsQueryService;
		_issueDetailRepository = issueDetailRepository;
		_storageService = storageService;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<ShadowingAttemptAssessmentsResponseDto>> Handle(
		GetShadowingAttemptAssessmentsQuery request,
		CancellationToken cancellationToken)
	{
		if (_currentUserContext.UserId is null)
		{
			return Result.Failure<ShadowingAttemptAssessmentsResponseDto>(ShadowingSessionErrors.Unauthenticated);
		}

		var ctx = await _analyticsQueryService.GetAttemptAssessmentsContextAsync(
			ShadowingAttemptId.From(request.AttemptId),
			cancellationToken);

		if (ctx is null || ctx.SessionId != request.SessionId)
		{
			return Result.Failure<ShadowingAttemptAssessmentsResponseDto>(ShadowingAttemptErrors.NotFound);
		}

		var isCurator = AttemptMapper.IsCurator(_currentUserContext);
		if (!isCurator && UserId.From(ctx.MemberId) != _currentUserContext.UserId)
		{
			return Result.Failure<ShadowingAttemptAssessmentsResponseDto>(ShadowingAttemptErrors.NotOwner);
		}

		var presignedUrls = await PresignAudioKeysAsync(ctx.Lines, cancellationToken);

		var orderedAssessments = ctx.Assessments
			.OrderByDescending(a => a.Type == ShadowingAssessmentType.CuratorReevaluation)
			.ThenByDescending(a => a.CreatedAt)
			.ToList();

		var mappedAssessments = orderedAssessments
			.Select(assessment => MapAssessmentItem(assessment, ctx.Lines, presignedUrls))
			.ToList();

		var primaryAssessment = mappedAssessments.FirstOrDefault();
		var topLevelLines = primaryAssessment?.Lines ?? BuildUnassessedLines(ctx.Lines, presignedUrls);

		int completedLinesCount = ctx.Lines.Count(l => !l.IsSkipped);
		int skippedLinesCount = ctx.Lines.Count(l => l.IsSkipped);
		int totalRecordedSeconds = (int)Math.Round(
			ctx.Lines.Where(l => !l.IsSkipped).Sum(l => l.DurationMs ?? 0L) / 1000.0);

		var latestReevalIssue = await _issueDetailRepository.GetLatestByTargetAndCategoryAsync(
			TargetType.ShadowingAttempt,
			ctx.AttemptId,
			CurationContextHelper.ShadowingReevaluationCategory,
			UserId.From(ctx.MemberId),
			cancellationToken);

		string? reevaluationRequestStatus = latestReevalIssue is not null
			? CurationContextHelper.MapLearnerIssueStatus(latestReevalIssue.Report)
			: null;
		Guid? reevaluationIssueId = latestReevalIssue?.Id.Value;
		Guid? reevaluationReportId = latestReevalIssue?.ReportId?.Value;

		return Result.Success(new ShadowingAttemptAssessmentsResponseDto(
			ctx.AttemptId,
			ctx.SessionId,
			ctx.MaterialName,
			ctx.MaterialJapaneseName,
			ctx.VideoTitle,
			ctx.RoleName,
			ctx.AttemptStatus.ToString(),
			ctx.StartedAt,
			ctx.CompletedAt,
			totalRecordedSeconds,
			completedLinesCount,
			skippedLinesCount,
			primaryAssessment,
			mappedAssessments,
			topLevelLines,
			reevaluationRequestStatus,
			reevaluationIssueId,
			reevaluationReportId));
	}

	private async Task<Dictionary<string, string?>> PresignAudioKeysAsync(
		IReadOnlyList<ShadowingAttemptLineContextModel> lines,
		CancellationToken cancellationToken)
	{
		var urls = new Dictionary<string, string?>(StringComparer.Ordinal);
		var keys = lines
			.SelectMany(l => new[] { l.UserAudioKey, l.ReferenceAudioKey })
			.Where(k => !string.IsNullOrWhiteSpace(k))
			.Select(k => k!)
			.Distinct(StringComparer.Ordinal);

		foreach (var key in keys)
		{
			urls[key] = await _storageService.CreatePresignedReadUrlAsync(key, cancellationToken);
		}

		return urls;
	}

	private static ShadowingAttemptAssessmentItemDto MapAssessmentItem(
		ShadowingAssessmentContextModel assessment,
		IReadOnlyList<ShadowingAttemptLineContextModel> lines,
		IReadOnlyDictionary<string, string?> presignedUrls)
	{
		var detailByLine = assessment.Details.ToDictionary(d => d.AttemptDetailId);
		var mappedLines = MapLinesWithDetails(lines, detailByLine, presignedUrls);

		var accScores = mappedLines.Where(l => l.AccuracyScore.HasValue).Select(l => l.AccuracyScore!.Value).ToList();
		var pitchScores = mappedLines.Where(l => l.PitchSimilarityScore.HasValue).Select(l => l.PitchSimilarityScore!.Value).ToList();

		decimal? avgAcc = accScores.Count > 0 ? Math.Round(accScores.Average(), 1) : null;
		decimal? avgPitch = pitchScores.Count > 0 ? Math.Round(pitchScores.Average(), 1) : null;
		decimal? overall = assessment.OverallScore.HasValue
			? Math.Round(assessment.OverallScore.Value, 1)
			: mappedLines.Any(l => l.AccuracyScore.HasValue || l.PitchSimilarityScore.HasValue)
				? Math.Round(
					mappedLines
						.Where(l => l.AccuracyScore.HasValue || l.PitchSimilarityScore.HasValue)
						.Average(l => ((l.AccuracyScore ?? 0m) + (l.PitchSimilarityScore ?? 0m)) / 2m),
					1)
				: null;

		return new ShadowingAttemptAssessmentItemDto(
			assessment.AssessmentId,
			assessment.Type.ToString(),
			overall,
			avgAcc,
			avgPitch,
			assessment.OverallFeedback,
			assessment.CreatedAt,
			mappedLines);
	}

	private static IReadOnlyList<ShadowingAttemptLineAssessmentDto> BuildUnassessedLines(
		IReadOnlyList<ShadowingAttemptLineContextModel> lines,
		IReadOnlyDictionary<string, string?> presignedUrls)
	{
		var empty = new Dictionary<Guid, ShadowingAssessmentDetailContextModel>();
		return MapLinesWithDetails(lines, empty, presignedUrls);
	}

	private static IReadOnlyList<ShadowingAttemptLineAssessmentDto> MapLinesWithDetails(
		IReadOnlyList<ShadowingAttemptLineContextModel> lines,
		IReadOnlyDictionary<Guid, ShadowingAssessmentDetailContextModel> detailByAttemptDetailId,
		IReadOnlyDictionary<string, string?> presignedUrls)
	{
		return lines
			.OrderBy(l => l.SortOrder)
			.ThenBy(l => l.SubmittedAt)
			.Select(line =>
			{
				detailByAttemptDetailId.TryGetValue(line.AttemptDetailId, out var detail);
				var userUrl = !string.IsNullOrWhiteSpace(line.UserAudioKey)
					? presignedUrls.GetValueOrDefault(line.UserAudioKey)
					: null;
				var refUrl = !string.IsNullOrWhiteSpace(line.ReferenceAudioKey)
					? presignedUrls.GetValueOrDefault(line.ReferenceAudioKey)
					: null;
				var words = detail is not null
					? AzureSpeechWeakWordParser.ParseWordBreakdown(detail.AccuracyAssessmentResponse)
					: [];

				return new ShadowingAttemptLineAssessmentDto(
					line.AttemptDetailId,
					line.RoleLineId,
					line.SortOrder,
					line.LineText,
					line.VietnameseMeaning,
					line.StartTime,
					line.EndTime,
					line.DurationMs,
					line.IsSkipped,
					line.SubmittedAt,
					userUrl,
					refUrl,
					detail?.AccuracyScore.HasValue == true ? Math.Round(detail.AccuracyScore.Value, 1) : null,
					detail?.PitchSimilarityScore.HasValue == true ? Math.Round(detail.PitchSimilarityScore.Value, 1) : null,
					detail?.Feedback,
					words);
			})
			.ToList();
	}
}
