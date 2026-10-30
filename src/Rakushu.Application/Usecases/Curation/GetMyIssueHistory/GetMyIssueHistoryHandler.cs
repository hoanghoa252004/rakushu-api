using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.Curation.GetMyIssueHistory;

internal sealed class GetMyIssueHistoryHandler : IRequestHandler<GetMyIssueHistoryQuery, Result<IReadOnlyList<LearnerIssueHistoryDto>>>
{
	private readonly IIssueDetailRepository _issueDetailRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetMyIssueHistoryHandler(
		IIssueDetailRepository issueDetailRepository,
		ICurrentUserContext currentUserContext)
	{
		_issueDetailRepository = issueDetailRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<IReadOnlyList<LearnerIssueHistoryDto>>> Handle(
		GetMyIssueHistoryQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<IReadOnlyList<LearnerIssueHistoryDto>>(UserErrors.NotFound);

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var issues = await _issueDetailRepository.GetByUserIdAsync(userId, page, pageSize, cancellationToken);

		var dtos = issues.Select(x => new LearnerIssueHistoryDto(
			x.Id.Value,
			x.TargetType.ToString(),
			x.TargetId,
			x.Category,
			x.LearnerFeedback,
			x.Report is null ? "PendingThreshold" : x.Report.Status.ToString(),
			x.CreatedAt)).ToList();

		return Result.Success<IReadOnlyList<LearnerIssueHistoryDto>>(dtos);
	}
}
