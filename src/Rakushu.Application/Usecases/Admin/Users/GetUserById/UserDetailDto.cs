namespace Rakushu.Application.Usecases.Admin.Users.GetUserById;

public sealed record UserDetailDto(
	Guid UserId,
	string Username,
	string Email,
	Guid RoleId,
	string RoleName,
	string DisplayName,
	string? AvatarUrl,
	string? Bio,
	string? NativeLanguage,
	string? LearningLanguage,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);
