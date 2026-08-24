using MediatR;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.UpdateUser;

public sealed record AdminUpdateUserCommand(
	Guid UserId,
	string Username,
	string Email,
	Guid RoleId,
	string Status,
	string DisplayName,
	string? AvatarUrl = null,
	string? Bio = null,
	string? NativeLanguage = null,
	string? LearningLanguage = null
) : IRequest<Result<UserDetailDto>>;
