using MediatR;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.CreateUser;

public sealed record AdminCreateUserCommand(
	string Username,
	string Email,
	string Password,
	Guid RoleId,
	string? DisplayName = null,
	string? Status = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
) : IRequest<Result<UserDetailDto>>;
