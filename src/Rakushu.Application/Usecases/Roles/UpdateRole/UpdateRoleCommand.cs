using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Roles.UpdateRole;

public record UpdateRoleCommand(
	Guid RoleId,
	string? Description = null,
	bool? IsActive = null
) : IRequest<Result>;
