namespace Rakushu.Application.Usecases.Admin.Roles.GetRoles;

public sealed record RoleDto(
	Guid RoleId,
	string RoleName,
	string? Description,
	DateTimeOffset CreatedAt
);
