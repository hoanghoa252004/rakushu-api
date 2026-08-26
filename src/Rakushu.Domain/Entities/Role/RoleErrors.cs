using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Role;

public static class RoleErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"ROLE.NOT_FOUND", "The specified role was not found.");
}
