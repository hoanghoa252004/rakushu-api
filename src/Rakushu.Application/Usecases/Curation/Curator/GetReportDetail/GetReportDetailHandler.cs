using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Curator.GetReportDetail;

internal sealed class GetReportDetailHandler : IRequestHandler<GetReportDetailQuery, Result<CuratorReportDetailDto>>
{
	private readonly IReportRepository _reportRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerAnalyticsQueryService _learnerAnalyticsQueryService;
	private readonly IStorageService _storageService;

	public GetReportDetailHandler(
		IReportRepository reportRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerAnalyticsQueryService learnerAnalyticsQueryService,
		IStorageService storageService)
	{
		_reportRepository = reportRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_learnerAnalyticsQueryService = learnerAnalyticsQueryService;
		_storageService = storageService;
	}

	public async Task<Result<CuratorReportDetailDto>> Handle(
		GetReportDetailQuery request,
		CancellationToken cancellationToken)
	{
		var reportId = ReportId.From(request.ReportId);
		var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
		if (report is null)
		{
			return Result.Failure<CuratorReportDetailDto>(ReportErrors.NotFound);
		}

		var feedbacks = report.Details.Select(d => new IssueFeedbackDto(
			d.UserId.Value,
			d.User?.FullName,
			d.LearnerFeedback,
			d.CreatedAt)).ToList();

		BankQuestionContextDto? questionContext = null;
		VideoLearningUnitContextDto? videoContext = null;
		ShadowingAttemptContextDto? shadowingContext = null;

		try
		{
			if (report.TargetType == TargetType.BankQuestion)
			{
				var q = await _bankQuestionRepository.GetByIdWithDetailsAsync(
					BankQuestionId.From(report.TargetId), cancellationToken);
				if (q is not null)
				{
					questionContext = new BankQuestionContextDto(
						q.Id.Value,
						q.Question,
						q.QuestionType?.Code ?? "SINGLE_CHOICE",
						q.Answers,
						q.CorrectAnswer,
						q.Explanation,
						q.KnowledgeMeaning?.KnowledgeUnit?.Expression,
						q.KnowledgeMeaning?.KnowledgeUnit?.Reading,
						q.KnowledgeMeaning?.Meaning,
						q.IsActive,
						q.MediaType,
						q.MediaUrl,
						q.MediaMetadata,
						q.UpdatedAt);
				}
			}
			else if (report.TargetType == TargetType.TranscriptSegment)
			{
				var segment = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
					TranscriptSegmentId.From(report.TargetId), cancellationToken);
				if (segment is not null)
				{
					var video = segment.Transcript?.Video;
					double startTimeSecs = segment.StartTime.TotalSeconds;
					double endTimeSecs = segment.EndTime.TotalSeconds;
					double durationSecs = video?.Duration.TotalSeconds ?? 0;

					var (videoUrl, thumbnailUrl) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
						video,
						_storageService,
						includeThumbnail: true,
						cancellationToken);

					var oovText = CurationContextHelper.ExtractOovText(
						segment.Text,
						report.StartCharIndex,
						report.EndCharIndex);

					var isOovReport = string.Equals(report.Category, CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase);
					var matchedOovUnit = isOovReport
						? CurationContextHelper.FindMatchingLearningUnitForOov(
							segment,
							report.StartCharIndex,
							report.EndCharIndex,
							oovText)
						: null;

					var transcriptSpeaker = segment.TranscriptSpeaker;
					var speaker = transcriptSpeaker?.Speaker;
					var speakerAvatarUrl = await CurationContextHelper.ResolveSpeakerAvatarUrlAsync(
						speaker?.AvatarKey,
						_storageService,
						cancellationToken);

					videoContext = new VideoLearningUnitContextDto(
						matchedOovUnit?.Id.Value,
						video?.Id.Value ?? Guid.Empty,
						video?.Title ?? "Video bài học",
						videoUrl,
						thumbnailUrl,
						durationSecs,
						CurationContextHelper.FormatTimestamp(durationSecs),
						startTimeSecs,
						endTimeSecs,
						startTimeSecs,
						CurationContextHelper.FormatTimestamp(startTimeSecs),
						segment.Text ?? string.Empty,
						segment.Subtitle?.Text,
						oovText,
						report.StartCharIndex ?? 0,
						report.EndCharIndex ?? 0,
						matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Expression,
						matchedOovUnit?.KnowledgeMeaning?.Meaning,
						matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Reading,
						video?.SourceType.ToString(),
						TranscriptSpeakerId: segment.TranscriptSpeakerId?.Value,
						SpeakerId: transcriptSpeaker?.SpeakerId?.Value,
						SpeakerLabel: transcriptSpeaker?.Label,
						SpeakerName: speaker?.Name,
						SpeakerJapaneseName: speaker?.JapaneseName,
						SpeakerThemeColor: speaker?.ThemeColor,
						SpeakerAvatarUrl: speakerAvatarUrl);
				}
			}
			else if (report.TargetType == TargetType.LearningUnit)
			{
				var (unit, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
					LearningUnitId.From(report.TargetId), cancellationToken);
				if (unit is not null)
				{
					var segment = unit.TranscriptSegment;
					var video = segment?.Transcript?.Video;
					double startTimeSecs = segment?.StartTime.TotalSeconds ?? 0;
					double endTimeSecs = segment?.EndTime.TotalSeconds ?? 0;
					double durationSecs = video?.Duration.TotalSeconds ?? 0;

					var (videoUrl, thumbnailUrl) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
						video,
						_storageService,
						includeThumbnail: true,
						cancellationToken);

					var targetToken = unit.KnowledgeMeaning?.KnowledgeUnit?.Expression;

					videoContext = new VideoLearningUnitContextDto(
						unit.Id.Value,
						video?.Id.Value ?? Guid.Empty,
						video?.Title ?? "Video bài học",
						videoUrl,
						thumbnailUrl,
						durationSecs,
						CurationContextHelper.FormatTimestamp(durationSecs),
						startTimeSecs,
						endTimeSecs,
						startTimeSecs,
						CurationContextHelper.FormatTimestamp(startTimeSecs),
						segment?.Text ?? string.Empty,
						segment?.Subtitle?.Text,
						targetToken,
						unit.StartTokenIndex,
						unit.EndTokenIndex,
						targetToken,
						unit.KnowledgeMeaning?.Meaning,
						unit.KnowledgeMeaning?.KnowledgeUnit?.Reading,
						video?.SourceType.ToString());
				}
			}
			else if (report.TargetType == TargetType.ShadowingAttempt)
			{
				var attemptCtx = await _learnerAnalyticsQueryService.GetAttemptAssessmentsContextAsync(
					ShadowingAttemptId.From(report.TargetId), cancellationToken);
				if (attemptCtx is not null)
				{
					shadowingContext = CurationContextHelper.BuildShadowingAttemptContext(attemptCtx);
				}
			}
		}
		catch
		{
			// Non-critical: Context enrichment failure should not block fetching the report ticket
		}

		var context = (questionContext is not null || videoContext is not null || shadowingContext is not null)
			? new ReportTargetContextDto(questionContext, videoContext, shadowingContext)
			: null;

		var reportedSnapshot = CurationContextHelper.ResolveCanonicalSnapshot(report);

		int? startCharIndex = report.StartCharIndex ?? reportedSnapshot?.StartCharIndex;
		int? endCharIndex = report.EndCharIndex ?? reportedSnapshot?.EndCharIndex;
		string? selectedOovText =
			reportedSnapshot?.SelectedOovText
			?? CurationContextHelper.ExtractOovText(
				videoContext?.OriginalSentenceJp ?? reportedSnapshot?.OriginalSentenceJp,
				startCharIndex,
				endCharIndex);
		string? videoSourceType = videoContext?.VideoSourceType ?? reportedSnapshot?.VideoSourceType;

		var dto = new CuratorReportDetailDto(
			report.Id.Value,
			report.TargetType.ToString(),
			report.TargetId,
			report.Category,
			report.Status.ToString(),
			report.CuratorNotes,
			report.CreatedAt,
			report.ResolvedAt,
			feedbacks,
			context,
			reportedSnapshot,
			startCharIndex,
			endCharIndex,
			selectedOovText,
			videoSourceType);

		return Result.Success(dto);
	}
}
