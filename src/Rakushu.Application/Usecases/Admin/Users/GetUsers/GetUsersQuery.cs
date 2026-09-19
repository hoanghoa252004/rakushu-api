using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

public sealed record GetUsersQuery(
	int PageNumber,
	int PageSize,
	string? SearchTerm = null,
	Guid? RoleId = null,
	string? Status = null
) : IRequest<Result<PaginatedList<UserDto>>>;
