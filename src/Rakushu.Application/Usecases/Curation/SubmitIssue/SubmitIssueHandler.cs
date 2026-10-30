using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Curation.SubmitIssue;

internal sealed class SubmitIssueHandler : IRequestHandler<SubmitIssueCommand, Result<SubmitIssueResponse>>
{
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly IReportRepository _reportRepository;
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitIssueHandler(
		IIssueCategoryRepository issueCategoryRepository,
		IReportRepository reportRepository,
		IIssueDetailRepository issueDetailRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_issueCategoryRepository = issueCategoryRepository;
		_reportRepository = reportRepository;
		_issueDetailRepository = issueDetailRepository;
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
				request.LearnerFeedback);

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
					var reportResult = Report.Create(targetType, request.TargetId, request.Category, now);
					if (reportResult.IsFailure)
						return Result.Failure<SubmitIssueResponse>(reportResult.Error);

					activeReport = reportResult.Value;
					_reportRepository.Add(activeReport);

					detail.AssignToReport(activeReport.Id);

					var unassignedDetails = await _issueDetailRepository.GetUnassignedByTargetAndCategoryAsync(
						targetType,
						request.TargetId,
						request.Category,
						cancellationToken);

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

			string status = thresholdReached ? "EscalatedToCurator" : "Logged";
			string message = thresholdReached
				? "Báo cáo của bạn và các người học khác đã đạt ngưỡng và được chuyển tới Curator để xử lý."
				: "Cảm ơn bạn đã phản hồi! Báo cáo đã được ghi nhận vào hệ thống.";

			return Result.Success(new SubmitIssueResponse(detail.Id.Value, status, message));
		}, cancellationToken);
	}
}
