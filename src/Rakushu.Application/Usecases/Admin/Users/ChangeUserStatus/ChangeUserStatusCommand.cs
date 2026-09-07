using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Usecases.Admin.Users.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(Guid UserId, UserStatus Status) : IRequest<Result>;
