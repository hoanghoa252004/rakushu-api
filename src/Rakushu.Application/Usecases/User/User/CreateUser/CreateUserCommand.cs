using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Application.Usecases.User.User.CreateUser;

public sealed record CreateUserCommand(
	string FullName,
	string Email,
	string Password,
	Guid RoleId
) : IRequest<Result<UserId>>;
