using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Learner.GetMyIssueHistory;

internal sealed class GetMyIssueHistoryHandler : IRequestHandler<GetMyIssueHistoryQuery, Result<PaginatedList<LearnerIssueHistoryDto>>>
{
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public GetMyIssueHistoryHandler(
		IIssueDetailRepository issueDetailRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_issueDetailRepository = issueDetailRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<PaginatedList<LearnerIssueHistoryDto>>> Handle(
		GetMyIssueHistoryQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<PaginatedList<LearnerIssueHistoryDto>>(UserErrors.NotFound);
		}

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		TargetType? targetType = null;
		if (!string.IsNullOrWhiteSpace(request.TargetType) &&
			Enum.TryParse<TargetType>(request.TargetType, true, out var parsedTargetType))
		{
			targetType = parsedTargetType;
		}

		var (issues, totalCount) = await _issueDetailRepository.GetByUserIdAsync(
			userId,
			page,
			pageSize,
			request.Status,
			targetType,
			cancellationToken);

		var dtos = new List<LearnerIssueHistoryDto>(issues.Count);
		foreach (var x in issues)
		{
			string reportStatus = x.Report switch
			{
				null => "Pending",
				{ Status: ReportStatus.Pending } => "Pending",
				{ Status: ReportStatus.InReview } => "Investigating",
				{ Status: ReportStatus.Resolved } => "Resolved",
				{ Status: ReportStatus.Dismissed } => "Dismissed",
				_ => x.Report.Status.ToString()
			};

			bool isFinalized = x.Report?.Status is ReportStatus.Resolved or ReportStatus.Dismissed;
			var reportedSnapshot = CurationContextHelper.DeserializeSnapshot(x.TargetSnapshot ?? x.Report?.TargetSnapshot);

			string? targetTitle;
			LearnerTargetContextDto? targetContext;

			if (!isFinalized && reportedSnapshot is not null && x.TargetType == TargetType.BankQuestion)
			{
				// Do not leak Curator WIP edits or soft-delete status to Learners before Resolve/Dismiss.
				(targetTitle, targetContext) = BuildSnapshotContext(x.TargetType, reportedSnapshot, videoUrl: null);
			}
			else
			{
				(targetTitle, targetContext) = await ResolveLearnerTargetContextAsync(x.TargetType, x.TargetId, cancellationToken);

				if (!isFinalized)
				{
					if (reportedSnapshot is not null)
					{
						(targetTitle, targetContext) = BuildSnapshotContext(x.TargetType, reportedSnapshot, targetContext?.VideoUrl);
					}
					else if (targetContext is not null && x.TargetType == TargetType.BankQuestion && targetContext.IsActive == false)
					{
						// Legacy pre-snapshot record: keep soft-delete hidden while report is still open.
						targetContext = targetContext with { IsActive = true };
					}
				}
			}

			if (string.IsNullOrWhiteSpace(targetTitle) && reportedSnapshot is not null)
			{
				targetTitle = CurationContextHelper.FormatSnapshotTitle(x.TargetType, reportedSnapshot);
			}

			dtos.Add(new LearnerIssueHistoryDto(
				x.Id.Value,
				x.TargetType.ToString(),
				x.TargetId,
				targetTitle,
				x.Category,
				x.LearnerFeedback,
				reportStatus,
				x.Report?.CuratorNotes,
				x.CreatedAt,
				x.Report?.ResolvedAt,
				targetContext,
				reportedSnapshot));
		}

		var pagedList = new PaginatedList<LearnerIssueHistoryDto>(dtos, totalCount, page, pageSize);

		return Result.Success(pagedList);
	}

	private static (string? TargetTitle, LearnerTargetContextDto Context) BuildSnapshotContext(
		TargetType targetType,
		CurationTargetSnapshotDto snap,
		string? videoUrl)
	{
		switch (targetType)
		{
			case TargetType.BankQuestion:
			{
				var qTitle = CurationContextHelper.FormatSnapshotTitle(targetType, snap);
				var qContext = new LearnerTargetContextDto(
					snap.QuestionTitle,
					snap.QuestionTypeCode ?? "SINGLE_CHOICE",
					snap.AnswersJson,
					snap.CorrectAnswer,
					snap.Explanation,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					null,
					snap.Expression,
					snap.Reading,
					snap.Meaning,
					IsActive: true);
				return (qTitle, qContext);
			}

			case TargetType.TranscriptSegment:
			{
				double startSecs = snap.StartTimeSeconds ?? 0;
				double endSecs = snap.EndTimeSeconds ?? 0;
				string ts = CurationContextHelper.FormatTimestamp(startSecs);
				var videoTitle = snap.VideoTitle ?? "Video bài học";
				var title = CurationContextHelper.FormatSnapshotTitle(targetType, snap)
					?? $"Video: {videoTitle} ({ts})";
				var ctx = new LearnerTargetContextDto(
					null, null, null, null, null,
					snap.VideoId,
					videoTitle,
					videoUrl,
					ts,
					startSecs,
					endSecs,
					snap.OriginalSentenceJp,
					snap.SubtitleVn,
					null, null, null, null, null, null);
				return (title, ctx);
			}

			case TargetType.LearningUnit:
			{
				double startSecs = snap.StartTimeSeconds ?? 0;
				double endSecs = snap.EndTimeSeconds ?? 0;
				string ts = CurationContextHelper.FormatTimestamp(startSecs);
				var videoTitle = snap.VideoTitle ?? "Video bài học";
				var token = snap.Expression ?? snap.TargetToken;
				var title = !string.IsNullOrWhiteSpace(token)
					? $"Video: {videoTitle} (Mốc {ts}) - Cụm 「{token}」"
					: $"Video: {videoTitle} (Mốc {ts})";
				var ctx = new LearnerTargetContextDto(
					null, null, null, null, null,
					snap.VideoId,
					videoTitle,
					videoUrl,
					ts,
					startSecs,
					endSecs,
					snap.OriginalSentenceJp,
					snap.SubtitleVn,
					snap.TargetToken ?? token,
					snap.StartTokenIndex,
					snap.EndTokenIndex,
					snap.Expression ?? token,
					snap.Reading,
					snap.Meaning);
				return (title, ctx);
			}

			default:
				return (CurationContextHelper.FormatSnapshotTitle(targetType, snap), new LearnerTargetContextDto(
					snap.QuestionTitle, snap.QuestionTypeCode, snap.AnswersJson, snap.CorrectAnswer, snap.Explanation,
					snap.VideoId, snap.VideoTitle, videoUrl, null, snap.StartTimeSeconds, snap.EndTimeSeconds,
					snap.OriginalSentenceJp, snap.SubtitleVn, snap.TargetToken, snap.StartTokenIndex, snap.EndTokenIndex,
					snap.Expression, snap.Reading, snap.Meaning, IsActive: true));
		}
	}

	private async Task<(string? TargetTitle, LearnerTargetContextDto? Context)> ResolveLearnerTargetContextAsync(
		TargetType targetType,
		Guid targetId,
		CancellationToken cancellationToken)
	{
		try
		{
			switch (targetType)
			{
				case TargetType.BankQuestion:
					var q = await _bankQuestionRepository.GetByIdWithDetailsAsync(
						BankQuestionId.From(targetId), cancellationToken);
					if (q is not null)
					{
						var title = $"Câu hỏi: {q.Question}";
						var context = new LearnerTargetContextDto(
							q.Question,
							q.QuestionType?.Code ?? "SINGLE_CHOICE",
							q.Answers,
							q.CorrectAnswer,
							q.Explanation,
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							q.KnowledgeMeaning?.KnowledgeUnit?.Expression,
							q.KnowledgeMeaning?.KnowledgeUnit?.Reading,
							q.KnowledgeMeaning?.Meaning,
							q.IsActive);
						return (title, context);
					}
					return (null, null);

				case TargetType.TranscriptSegment:
					var seg = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
						TranscriptSegmentId.From(targetId), cancellationToken);
					if (seg is not null)
					{
						var video = seg.Transcript?.Video;
						var videoTitle = video?.Title ?? "Video bài học";
						double startTimeSecs = seg.StartTime.TotalSeconds;
						double endTimeSecs = seg.EndTime.TotalSeconds;
						string timestampFormatted = CurationContextHelper.FormatTimestamp(startTimeSecs);

						var (videoUrl, _) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
							video,
							_storageService,
							includeThumbnail: false,
							cancellationToken);

						var title = $"Video: {videoTitle} ({timestampFormatted})";

						var context = new LearnerTargetContextDto(
							null,
							null,
							null,
							null,
							null,
							video?.Id.Value,
							videoTitle,
							videoUrl,
							timestampFormatted,
							startTimeSecs,
							endTimeSecs,
							seg.Text,
							seg.Subtitle?.Text,
							null,
							null,
							null,
							null,
							null,
							null);

						return (title, context);
					}
					return (null, null);

				case TargetType.LearningUnit:
					var (lu, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
						LearningUnitId.From(targetId), cancellationToken);
					if (lu is not null)
					{
						var segment = lu.TranscriptSegment;
						var video = segment?.Transcript?.Video;
						var videoTitle = video?.Title ?? "Video bài học";
						var token = lu.KnowledgeMeaning?.KnowledgeUnit?.Expression;
						double startTimeSecs = segment?.StartTime.TotalSeconds ?? 0;
						double endTimeSecs = segment?.EndTime.TotalSeconds ?? 0;
						string timestampFormatted = CurationContextHelper.FormatTimestamp(startTimeSecs);

						var (videoUrl, _) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
							video,
							_storageService,
							includeThumbnail: false,
							cancellationToken);

						var title = token is not null
							? $"Video: {videoTitle} (Mốc {timestampFormatted}) - Cụm 「{token}」"
							: $"Video: {videoTitle} (Mốc {timestampFormatted})";

						var context = new LearnerTargetContextDto(
							null,
							null,
							null,
							null,
							null,
							video?.Id.Value,
							videoTitle,
							videoUrl,
							timestampFormatted,
							startTimeSecs,
							endTimeSecs,
							segment?.Text,
							segment?.Subtitle?.Text,
							token,
							lu.StartTokenIndex,
							lu.EndTokenIndex,
							token,
							lu.KnowledgeMeaning?.KnowledgeUnit?.Reading,
							lu.KnowledgeMeaning?.Meaning);

						return (title, context);
					}
					return (null, null);

				default:
					return (null, null);
			}
		}
		catch
		{
			return (null, null);
		}
	}
}
