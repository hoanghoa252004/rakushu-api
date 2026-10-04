using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.Role.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyCollection<GetRolesDto>>>;
