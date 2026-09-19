using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Users.GetUserById;

public sealed record UserDto(
	Guid Id,
	string Email,
	string FullName,
	string Role,
	string Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	string? AvatarUrl = null,
	string? NativeLanguage = null
);
