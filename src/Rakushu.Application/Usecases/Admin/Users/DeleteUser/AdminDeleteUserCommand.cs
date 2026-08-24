using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.DeleteUser;

public sealed record AdminDeleteUserCommand(Guid UserId) : IRequest<Result>;
