using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
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

		string? targetSnapshotJson = null;

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

			targetSnapshotJson = CurationContextHelper.SerializeTranscriptSegment(seg);
		}
		else if (targetType == TargetType.LearningUnit)
		{
			var (lu, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
				LearningUnitId.From(request.TargetId),
				cancellationToken);

			if (lu is null)
				return Result.Failure<SubmitIssueResponse>(ReportErrors.TargetNotFound);

			targetSnapshotJson = CurationContextHelper.SerializeLearningUnit(lu);
		}

		var hasActive = await _issueDetailRepository.HasActiveIssueAsync(
			userId,
			targetType,
			request.TargetId,
			request.Category,
			cancellationToken);

		if (hasActive)
			return Result.Failure<SubmitIssueResponse>(IssueDetailErrors.AlreadyReported);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var detailResult = IssueDetail.Create(
				userId,
				targetType,
				request.TargetId,
				request.Category,
				now,
				request.LearnerFeedback,
				targetSnapshot: targetSnapshotJson);

			if (detailResult.IsFailure)
				return Result.Failure<SubmitIssueResponse>(detailResult.Error);

			var detail = detailResult.Value;

			var categoryAggregator = await _issueCategoryRepository.GetByTargetAndCategoryAsync(
				targetType,
				request.TargetId,
				request.Category,
				cancellationToken);

			bool thresholdReached;

			if (categoryAggregator is null)
			{
				var createCatResult = IssueCategory.Create(
					targetType,
					request.TargetId,
					request.Category,
					now,
					threshold: 3);

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
					request.Category,
					cancellationToken);

				if (activeReport is null)
				{
					var unassignedDetails = await _issueDetailRepository.GetUnassignedByTargetAndCategoryAsync(
						targetType,
						request.TargetId,
						request.Category,
						cancellationToken);

					var canonicalSnapshot = unassignedDetails
						.OrderBy(u => u.CreatedAt)
						.Select(u => u.TargetSnapshot)
						.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))
						?? targetSnapshotJson;

					var reportResult = Report.Create(
						targetType,
						request.TargetId,
						request.Category,
						now,
						targetSnapshot: canonicalSnapshot);

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
