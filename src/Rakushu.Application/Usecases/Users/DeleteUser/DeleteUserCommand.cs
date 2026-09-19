using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
