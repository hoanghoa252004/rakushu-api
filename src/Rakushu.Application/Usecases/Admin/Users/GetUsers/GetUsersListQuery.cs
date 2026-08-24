using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

public sealed record GetUsersListQuery(
	int PageNumber = 1,
	int PageSize = 10,
	string? SearchTerm = null,
	Guid? RoleId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<UserSummaryDto>>>;
