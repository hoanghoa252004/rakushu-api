using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.User.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;
