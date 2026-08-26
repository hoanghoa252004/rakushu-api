using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Repositories;

namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

internal sealed class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<PaginatedList<UserSummaryDto>>>
{
	private readonly IUserRepository _userRepository;

	public GetUsersListQueryHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public async Task<Result<PaginatedList<UserSummaryDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
	{
		var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
		var pageSize = request.PageSize <= 0 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

		var (items, totalCount) = await _userRepository.GetPagedAsync(
			pageNumber,
			pageSize,
			request.SearchTerm,
			request.RoleId,
			request.Status,
			cancellationToken);

		var dtos = items.Select(u => new UserSummaryDto(
			UserId: u.Id,
			Username: u.Username,
			Email: u.Email,
			RoleId: u.RoleId,
			RoleName: u.Role?.RoleName ?? "Learner",
			DisplayName: u.Profile?.DisplayName ?? u.Username,
			AvatarUrl: u.Profile?.AvatarUrl,
			Status: u.Status.ToString(),
			CreatedAt: u.CreatedAt,
			UpdatedAt: u.UpdatedAt
		)).ToList();

		var paginatedList = PaginatedList<UserSummaryDto>.Create(dtos, totalCount, pageNumber, pageSize);
		return Result.Success(paginatedList);
	}
}
