using MediatR;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.CreateUser;

public sealed record CreateUserCommand(
	string Email,
	string Password,
	Guid RoleId,
	string FullName,
	string? NativeLanguage,
	string? AvatarKey = null
) : IRequest<Result<UserId>>;