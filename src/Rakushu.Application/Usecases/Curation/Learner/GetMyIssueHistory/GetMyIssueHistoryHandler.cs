using System.Globalization;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Learner.GetMyIssueHistory;

internal sealed class GetMyIssueHistoryHandler : IRequestHandler<GetMyIssueHistoryQuery, Result<PaginatedList<LearnerIssueHistoryDto>>>
{
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerAnalyticsQueryService _learnerAnalyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;

	public GetMyIssueHistoryHandler(
		IIssueDetailRepository issueDetailRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerAnalyticsQueryService learnerAnalyticsQueryService,
		ICurrentUserContext currentUserContext,
		IStorageService storageService)
	{
		_issueDetailRepository = issueDetailRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_learnerAnalyticsQueryService = learnerAnalyticsQueryService;
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
			string reportStatus = CurationContextHelper.MapLearnerIssueStatus(x.Report);

			bool isFinalized = x.Report?.Status is ReportStatus.Resolved or ReportStatus.Dismissed;
			var reportedSnapshot = CurationContextHelper.DeserializeSnapshot(x.TargetSnapshot ?? x.Report?.TargetSnapshot);

			string? targetTitle;
			LearnerTargetContextDto? targetContext;

			if (!isFinalized && reportedSnapshot is not null &&
				x.TargetType is TargetType.BankQuestion or TargetType.ShadowingAttempt)
			{
				// Do not leak Curator WIP edits, soft-delete, or unfinalized re-evaluation to Learners before Resolve/Dismiss.
				(targetTitle, targetContext) = BuildSnapshotContext(x.TargetType, x.TargetId, reportedSnapshot, videoUrl: null);
			}
			else
			{
				bool isOovIssue = string.Equals(x.Category, CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase);
				bool maskUnfinalizedSpeaker = !isFinalized && reportedSnapshot is not null;
				(targetTitle, targetContext) = await ResolveLearnerTargetContextAsync(
					x.TargetType,
					x.TargetId,
					isOovIssue,
					x.StartCharIndex ?? reportedSnapshot?.StartCharIndex,
					x.EndCharIndex ?? reportedSnapshot?.EndCharIndex,
					reportedSnapshot?.SelectedOovText,
					maskUnfinalizedSpeaker,
					reportedSnapshot?.SpeakerId,
					cancellationToken);

				if (!isFinalized)
				{
					if (reportedSnapshot is not null)
					{
						var safeSpeakerAvatarUrl = targetContext?.SpeakerId is not null
							&& targetContext.SpeakerId == reportedSnapshot.SpeakerId
							? targetContext.SpeakerAvatarUrl
							: null;
						(targetTitle, targetContext) = BuildSnapshotContext(
							x.TargetType,
							x.TargetId,
							reportedSnapshot,
							targetContext?.VideoUrl,
							safeSpeakerAvatarUrl);
					}
					else if (targetContext is not null && x.TargetType == TargetType.BankQuestion && targetContext.IsActive == false)
					{
						// Legacy pre-snapshot record: keep soft-delete hidden while report is still open.
						targetContext = targetContext with { IsActive = true };
					}
					else if (targetContext is not null && x.TargetType == TargetType.ShadowingAttempt && targetContext.HasCuratorReevaluation == true)
					{
						// Legacy/corrupt snapshot fallback: keep unfinalized curator re-evaluation hidden while open.
						var baseline = targetContext.ShadowingContext?.AutomatedAssessment;
						var materialName = targetContext.ShadowingMaterialName ?? "Shadowing";
						var roleSuffix = !string.IsNullOrWhiteSpace(targetContext.ShadowingRoleName)
							? $" ({targetContext.ShadowingRoleName})"
							: string.Empty;
						var scoreSuffix = baseline?.OverallScore is decimal baselineScore
							? $" — {baselineScore.ToString("0.#", CultureInfo.InvariantCulture)} điểm"
							: string.Empty;
						targetTitle = $"Shadowing: {materialName}{roleSuffix}{scoreSuffix}";
						targetContext = targetContext with
						{
							ShadowingOverallScore = baseline?.OverallScore,
							ShadowingAccuracyScore = baseline?.AccuracyScore,
							ShadowingPitchSimilarityScore = baseline?.PitchSimilarityScore,
							ShadowingOverallFeedback = baseline?.OverallFeedback,
							HasCuratorReevaluation = false,
							ShadowingContext = targetContext.ShadowingContext is not null
								? targetContext.ShadowingContext with
								{
									HasCuratorReevaluation = false,
									CuratorAssessment = null
								}
								: null
						};
					}
				}
			}

			if (string.IsNullOrWhiteSpace(targetTitle) && reportedSnapshot is not null)
			{
				targetTitle = CurationContextHelper.FormatSnapshotTitle(x.TargetType, reportedSnapshot);
			}

			int? startCharIdx = x.StartCharIndex ?? reportedSnapshot?.StartCharIndex;
			int? endCharIdx = x.EndCharIndex ?? reportedSnapshot?.EndCharIndex;
			string? selectedOovText =
				reportedSnapshot?.SelectedOovText
				?? CurationContextHelper.ExtractOovText(
					targetContext?.OriginalSentenceJp ?? reportedSnapshot?.OriginalSentenceJp,
					startCharIdx,
					endCharIdx);

			if (targetContext is not null && startCharIdx.HasValue && endCharIdx.HasValue)
			{
				targetContext = targetContext with
				{
					TargetToken = targetContext.TargetToken ?? selectedOovText,
					StartTokenIndex = targetContext.StartTokenIndex ?? startCharIdx,
					EndTokenIndex = targetContext.EndTokenIndex ?? endCharIdx,
					StartCharIndex = startCharIdx,
					EndCharIndex = endCharIdx,
					SelectedOovText = selectedOovText,
					VideoSourceType = targetContext.VideoSourceType ?? reportedSnapshot?.VideoSourceType
				};
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
				reportedSnapshot,
				startCharIdx,
				endCharIdx,
				selectedOovText));
		}

		var pagedList = new PaginatedList<LearnerIssueHistoryDto>(dtos, totalCount, page, pageSize);

		return Result.Success(pagedList);
	}

	private static (string? TargetTitle, LearnerTargetContextDto Context) BuildSnapshotContext(
		TargetType targetType,
		Guid targetId,
		CurationTargetSnapshotDto snap,
		string? videoUrl,
		string? speakerAvatarUrl = null)
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
					snap.TargetToken,
					snap.StartTokenIndex,
					snap.EndTokenIndex,
					snap.Expression,
					snap.Reading,
					snap.Meaning,
					IsActive: null,
					StartCharIndex: snap.StartCharIndex,
					EndCharIndex: snap.EndCharIndex,
					SelectedOovText: snap.SelectedOovText,
					VideoSourceType: snap.VideoSourceType,
					TranscriptSpeakerId: snap.TranscriptSpeakerId,
					SpeakerId: snap.SpeakerId,
					SpeakerLabel: snap.SpeakerLabel,
					SpeakerName: snap.SpeakerName,
					SpeakerJapaneseName: snap.SpeakerJapaneseName,
					SpeakerThemeColor: snap.SpeakerThemeColor,
					SpeakerAvatarUrl: speakerAvatarUrl);
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
					snap.Meaning,
					IsActive: null,
					VideoSourceType: snap.VideoSourceType);
				return (title, ctx);
			}

			case TargetType.ShadowingAttempt:
			{
				var title = CurationContextHelper.FormatSnapshotTitle(targetType, snap);
				var ctx = new LearnerTargetContextDto(
					null, null, null, null, null,
					null, snap.VideoTitle, null, null, null, null,
					null, null, null, null, null, null, null, null,
					ShadowingSessionId: snap.ShadowingSessionId,
					ShadowingAttemptId: targetId,
					ShadowingMaterialName: snap.ShadowingMaterialName ?? snap.VideoTitle,
					ShadowingRoleName: snap.ShadowingRoleName,
					ShadowingOverallScore: snap.ShadowingOverallScore,
					ShadowingAccuracyScore: snap.ShadowingAccuracyScore,
					ShadowingPitchSimilarityScore: snap.ShadowingPitchSimilarityScore,
					ShadowingOverallFeedback: snap.ShadowingOverallFeedback,
					HasCuratorReevaluation: false);
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
		bool isOovIssue,
		int? startCharIndex,
		int? endCharIndex,
		string? snapshotOovText,
		bool maskUnfinalizedSpeaker,
		Guid? snapshotSpeakerId,
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

						var oovText = snapshotOovText
							?? CurationContextHelper.ExtractOovText(seg.Text, startCharIndex, endCharIndex);
						var matchedOovUnit = isOovIssue
							? CurationContextHelper.FindMatchingLearningUnitForOov(
								seg,
								startCharIndex,
								endCharIndex,
								oovText)
							: null;

						if (isOovIssue && !string.IsNullOrWhiteSpace(oovText))
						{
							title = $"Video: {videoTitle} ({timestampFormatted}) - OOV 「{oovText}」";
						}

						var transcriptSpeaker = seg.TranscriptSpeaker;
						var speaker = transcriptSpeaker?.Speaker;
						bool shouldPresignSpeakerAvatar = !maskUnfinalizedSpeaker
							|| (snapshotSpeakerId.HasValue && transcriptSpeaker?.SpeakerId?.Value == snapshotSpeakerId.Value);
						var speakerAvatarUrl = shouldPresignSpeakerAvatar
							? await CurationContextHelper.ResolveSpeakerAvatarUrlAsync(
								speaker?.AvatarKey,
								_storageService,
								cancellationToken)
							: null;

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
							isOovIssue ? oovText : null,
							isOovIssue ? startCharIndex : null,
							isOovIssue ? endCharIndex : null,
							matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Expression,
							matchedOovUnit?.KnowledgeMeaning?.KnowledgeUnit?.Reading,
							matchedOovUnit?.KnowledgeMeaning?.Meaning,
							IsActive: null,
							StartCharIndex: isOovIssue ? startCharIndex : null,
							EndCharIndex: isOovIssue ? endCharIndex : null,
							SelectedOovText: isOovIssue ? oovText : null,
							VideoSourceType: video?.SourceType.ToString(),
							TranscriptSpeakerId: seg.TranscriptSpeakerId?.Value,
							SpeakerId: transcriptSpeaker?.SpeakerId?.Value,
							SpeakerLabel: transcriptSpeaker?.Label,
							SpeakerName: speaker?.Name,
							SpeakerJapaneseName: speaker?.JapaneseName,
							SpeakerThemeColor: speaker?.ThemeColor,
							SpeakerAvatarUrl: speakerAvatarUrl);

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
							lu.KnowledgeMeaning?.Meaning,
							IsActive: null,
							VideoSourceType: video?.SourceType.ToString());

						return (title, context);
					}
					return (null, null);

				case TargetType.ShadowingAttempt:
					var attemptCtx = await _learnerAnalyticsQueryService.GetAttemptAssessmentsContextAsync(
						ShadowingAttemptId.From(targetId), cancellationToken);
					if (attemptCtx is not null)
					{
						var shadowingContext = CurationContextHelper.BuildShadowingAttemptContext(attemptCtx);
						var primary = shadowingContext.CuratorAssessment ?? shadowingContext.AutomatedAssessment;
						var roleSuffix = !string.IsNullOrWhiteSpace(shadowingContext.RoleName)
							? $" ({shadowingContext.RoleName})"
							: string.Empty;
						var scoreSuffix = primary?.OverallScore is decimal score
							? $" — {score.ToString("0.#", CultureInfo.InvariantCulture)} điểm"
							: string.Empty;
						var title = $"Shadowing: {shadowingContext.MaterialName}{roleSuffix}{scoreSuffix}";

						var context = new LearnerTargetContextDto(
							null, null, null, null, null,
							null, shadowingContext.VideoTitle, null, null, null, null,
							null, null, null, null, null, null, null, null,
							ShadowingSessionId: shadowingContext.SessionId,
							ShadowingAttemptId: shadowingContext.AttemptId,
							ShadowingMaterialName: shadowingContext.MaterialName,
							ShadowingRoleName: shadowingContext.RoleName,
							ShadowingOverallScore: primary?.OverallScore,
							ShadowingAccuracyScore: primary?.AccuracyScore,
							ShadowingPitchSimilarityScore: primary?.PitchSimilarityScore,
							ShadowingOverallFeedback: primary?.OverallFeedback,
							HasCuratorReevaluation: shadowingContext.HasCuratorReevaluation,
							ShadowingContext: shadowingContext);

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
