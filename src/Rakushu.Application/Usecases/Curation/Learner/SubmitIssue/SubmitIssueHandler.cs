using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Learner.SubmitIssue;

internal sealed class SubmitIssueHandler : IRequestHandler<SubmitIssueCommand, Result<SubmitIssueResponse>>
{
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly IReportRepository _reportRepository;
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerAnalyticsQueryService _learnerAnalyticsQueryService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitIssueHandler(
		IIssueCategoryRepository issueCategoryRepository,
		IReportRepository reportRepository,
		IIssueDetailRepository issueDetailRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerAnalyticsQueryService learnerAnalyticsQueryService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_issueCategoryRepository = issueCategoryRepository;
		_reportRepository = reportRepository;
		_issueDetailRepository = issueDetailRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_learnerAnalyticsQueryService = learnerAnalyticsQueryService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<SubmitIssueResponse>> Handle(
		SubmitIssueCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<SubmitIssueResponse>(UserErrors.NotFound);

		if (!Enum.TryParse<TargetType>(request.TargetType, true, out var targetType))
			return Result.Failure<SubmitIssueResponse>(ReportErrors.InvalidTargetId);

		bool isOov = string.Equals(request.Category?.Trim(), CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase);
		bool isWrongSpeaker = string.Equals(request.Category?.Trim(), CurationContextHelper.WrongSpeakerCategory, StringComparison.OrdinalIgnoreCase);
		bool isShadowingReeval = string.Equals(request.Category?.Trim(), CurationContextHelper.ShadowingReevaluationCategory, StringComparison.OrdinalIgnoreCase);
		string normalizedCategory = isOov
			? CurationContextHelper.OovCategory
			: isWrongSpeaker
				? CurationContextHelper.WrongSpeakerCategory
				: isShadowingReeval
					? CurationContextHelper.ShadowingReevaluationCategory
					: request.Category!.Trim();
		int? startCharIndex = isOov ? request.StartCharIndex : null;
		int? endCharIndex = isOov ? request.EndCharIndex : null;

		string? targetSnapshotJson = null;
		int effectiveThreshold = 3;

		if (targetType == TargetType.BankQuestion)
		{
			var bankQuestion = await _bankQuestionRepository.GetByIdWithDetailsAsync(
				BankQuestionId.From(request.TargetId),
				cancellationToken);

			if (bankQuestion is null)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			targetSnapshotJson = CurationContextHelper.SerializeBankQuestion(bankQuestion);
		}
		else if (targetType == TargetType.TranscriptSegment)
		{
			var seg = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
				TranscriptSegmentId.From(request.TargetId),
				cancellationToken);

			if (seg is null)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			var video = seg.Transcript?.Video;
			bool isPersonalVideo = video is not null && video.SourceType != VideoSource.Curated;
			if (isPersonalVideo && video!.LearnerId != userId)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			if (isOov)
			{
				var selectedText = CurationContextHelper.ExtractOovText(seg.Text, startCharIndex, endCharIndex);
				if (string.IsNullOrWhiteSpace(selectedText))
					return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.InvalidCharRange);
			}

			if (isPersonalVideo)
			{
				effectiveThreshold = 1;
			}

			targetSnapshotJson = CurationContextHelper.SerializeTranscriptSegment(seg, startCharIndex, endCharIndex);
		}
		else if (targetType == TargetType.LearningUnit)
		{
			var (lu, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
				LearningUnitId.From(request.TargetId),
				cancellationToken);

			if (lu is null)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			var video = lu.TranscriptSegment?.Transcript?.Video;
			bool isPersonalVideo = video is not null && video.SourceType != VideoSource.Curated;
			if (isPersonalVideo && video!.LearnerId != userId)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			if (isPersonalVideo)
			{
				effectiveThreshold = 1;
			}

			targetSnapshotJson = CurationContextHelper.SerializeLearningUnit(lu);
		}
		else if (targetType == TargetType.ShadowingAttempt)
		{
			var attemptCtx = await _learnerAnalyticsQueryService.GetAttemptAssessmentsContextAsync(
				ShadowingAttemptId.From(request.TargetId),
				cancellationToken);

			if (attemptCtx is null || attemptCtx.MemberId != userId.Value)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			if (attemptCtx.AttemptStatus != ShadowingAttemptStatus.Assessed || attemptCtx.Assessments.Count == 0)
				return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.ShadowingAttemptNotAssessed);

			if (attemptCtx.Assessments.Count >= 2 ||
				attemptCtx.Assessments.Any(a => a.Type == ShadowingAssessmentType.CuratorReevaluation))
			{
				return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.ShadowingAlreadyReevaluated);
			}

			if (!attemptCtx.Lines.Any(l => !l.IsSkipped))
				return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.ShadowingNoAssessedLines);

			effectiveThreshold = 1;
			targetSnapshotJson = CurationContextHelper.SerializeShadowingAttempt(attemptCtx);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			if (targetType == TargetType.ShadowingAttempt)
			{
				var hasAnyReevalIssue = await _issueDetailRepository.HasAnyIssueAsync(
					userId,
					targetType,
					request.TargetId,
					normalizedCategory,
					cancellationToken);

				if (hasAnyReevalIssue)
					return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.ShadowingReevaluationRequestLimitReached);
			}
			else
			{
				var hasActive = await _issueDetailRepository.HasActiveIssueAsync(
					userId,
					targetType,
					request.TargetId,
					normalizedCategory,
					startCharIndex,
					endCharIndex,
					cancellationToken);

				if (hasActive)
					return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.AlreadyReported);
			}

			var now = _systemClock.UtcNow;

			var detailResult = IssueDetail.Create(
				userId,
				targetType,
				request.TargetId,
				normalizedCategory,
				now,
				request.LearnerFeedback,
				targetSnapshot: targetSnapshotJson,
				startCharIndex: startCharIndex,
				endCharIndex: endCharIndex);

			if (detailResult.IsFailure)
				return Result.Failure<SubmitIssueResponse>(detailResult.Error);

			var detail = detailResult.Value;

			var categoryAggregator = await _issueCategoryRepository.GetByTargetAndCategoryAsync(
				targetType,
				request.TargetId,
				normalizedCategory,
				startCharIndex,
				endCharIndex,
				cancellationToken);

			bool thresholdReached;

			if (categoryAggregator is null)
			{
				var createCatResult = IssueCategory.Create(
					targetType,
					request.TargetId,
					normalizedCategory,
					now,
					threshold: effectiveThreshold,
					startCharIndex: startCharIndex,
					endCharIndex: endCharIndex);

				if (createCatResult.IsFailure)
					return Result.Failure<SubmitIssueResponse>(createCatResult.Error);

				categoryAggregator = createCatResult.Value;
				_issueCategoryRepository.Add(categoryAggregator);
				thresholdReached = categoryAggregator.CurrentCount >= categoryAggregator.Threshold;
			}
			else
			{
				thresholdReached = categoryAggregator.IncrementAndCheckThreshold(now);
			}

			if (thresholdReached)
			{
				var activeReport = await _reportRepository.GetActiveReportByTargetAndCategoryAsync(
					targetType,
					request.TargetId,
					normalizedCategory,
					startCharIndex,
					endCharIndex,
					cancellationToken);

				if (activeReport is null)
				{
					var unassignedDetails = await _issueDetailRepository.GetUnassignedByTargetAndCategoryAsync(
						targetType,
						request.TargetId,
						normalizedCategory,
						startCharIndex,
						endCharIndex,
						cancellationToken);

					var canonicalSnapshot = unassignedDetails
						.OrderBy(u => u.CreatedAt)
						.Select(u => u.TargetSnapshot)
						.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))
						?? targetSnapshotJson;

					var reportResult = Report.Create(
						targetType,
						request.TargetId,
						normalizedCategory,
						now,
						targetSnapshot: canonicalSnapshot,
						startCharIndex: startCharIndex,
						endCharIndex: endCharIndex);

					if (reportResult.IsFailure)
						return Result.Failure<SubmitIssueResponse>(reportResult.Error);

					activeReport = reportResult.Value;
					_reportRepository.Add(activeReport);

					detail.AssignToReport(activeReport.Id);

					foreach (var unassigned in unassignedDetails)
					{
						unassigned.AssignToReport(activeReport.Id);
					}
				}
				else
				{
					detail.AssignToReport(activeReport.Id);
				}
			}

			_issueDetailRepository.Add(detail);

			return Result.Success(new SubmitIssueResponse(detail.Id.Value));
		}, cancellationToken);
	}
}
