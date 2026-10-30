using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Curation.DeleteMyIssue;

internal sealed class DeleteMyIssueHandler : IRequestHandler<DeleteMyIssueCommand, Result>
{
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteMyIssueHandler(
		IIssueDetailRepository issueDetailRepository,
		IIssueCategoryRepository issueCategoryRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_issueDetailRepository = issueDetailRepository;
		_issueCategoryRepository = issueCategoryRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DeleteMyIssueCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		var issueId = IssueDetailId.From(request.IssueId);
		var issue = await _issueDetailRepository.GetByIdWithReportAsync(issueId, cancellationToken);
		if (issue is null || issue.UserId != userId)
			return Result.Failure(IssueDetailErrors.NotFound);

		if (issue.Report is not null &&
			(issue.Report.Status == ReportStatus.InReview ||
			 issue.Report.Status == ReportStatus.Resolved ||
			 issue.Report.Status == ReportStatus.Dismissed))
		{
			return Result.Failure(IssueDetailErrors.CannotDeleteUnderReview);
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			if (issue.ReportId is null)
			{
				var category = await _issueCategoryRepository.GetByTargetAndCategoryAsync(
					issue.TargetType,
					issue.TargetId,
					issue.Category,
					cancellationToken);

				category?.DecrementCount(now);
			}

			_issueDetailRepository.Delete(issue);

			return Result.Success();
		}, cancellationToken);
	}
}
