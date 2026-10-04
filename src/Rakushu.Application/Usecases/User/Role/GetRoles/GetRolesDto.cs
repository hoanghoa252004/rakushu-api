using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.User.Role.GetRoles;

public sealed record GetRolesDto(
	Guid Id,
	string Code,
	string Name,
	string? Description,
	bool isActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);
