using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Queries.User;

internal sealed record UserListRow(
	Guid Id,
	string Email,
	string Status,
	string? FullName,
	string? Avatar,
	DateTime CreatedAt,
	DateTime UpdatedAt,

	// Role data
	Guid RoleId,
	string RoleCode,
	string RoleName
);