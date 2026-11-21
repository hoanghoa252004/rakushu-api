using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Curation.Learner.GetMyIssueSummary;

internal sealed class GetMyIssueSummaryHandler : IRequestHandler<GetMyIssueSummaryQuery, Result<LearnerIssueSummaryDto>>
{
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetMyIssueSummaryHandler(
		IIssueDetailRepository issueDetailRepository,
		ICurrentUserContext currentUserContext)
	{
		_issueDetailRepository = issueDetailRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<LearnerIssueSummaryDto>> Handle(
		GetMyIssueSummaryQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<LearnerIssueSummaryDto>(UserErrors.NotFound);
		}

		var (total, pending, investigating, resolved, dismissed) =
			await _issueDetailRepository.GetSummaryByUserIdAsync(userId, cancellationToken);

		return Result.Success(new LearnerIssueSummaryDto(
			total,
			pending,
			investigating,
			resolved,
			dismissed));
	}
}
