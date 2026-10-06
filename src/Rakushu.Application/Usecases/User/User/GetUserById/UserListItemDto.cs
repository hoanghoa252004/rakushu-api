using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

public sealed record UserListItemDto(
	Guid Id,
	string FullName,
	string Email,
	string Status,
	string? Avatar,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	RoleDto Role
);

