using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Roles.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyCollection<RoleDto>>>;
