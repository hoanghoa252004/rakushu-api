using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Roles.GetRoles;

public sealed record GetRolesDto(
	Guid Id,
	string Title,
	string? Description,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt
);
