namespace Rakushu.Application.Usecases.Admin.Users.GetUsers;

public sealed record UserSummaryDto(
	Guid UserId,
	string Username,
	string Email,
	Guid RoleId,
	string RoleName,
	string DisplayName,
	string? AvatarUrl,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);
