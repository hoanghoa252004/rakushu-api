using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Roles.UpdateRole;

public record UpdateRoleCommand(
	Guid RoleId,
	string? Description = null
) : IRequest<Result>;
