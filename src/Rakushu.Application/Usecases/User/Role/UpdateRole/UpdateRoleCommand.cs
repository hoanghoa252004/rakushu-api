using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.Role.UpdateRole;

public record UpdateRoleCommand(
	Guid RoleId,
	string Name,
	bool IsActive,
	string? Description = null
) : IRequest<Result>;
