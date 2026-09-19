using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Users.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(Guid UserId, string Status) : IRequest<Result>;
