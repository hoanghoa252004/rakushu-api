using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.ChangeStatus;

public sealed record AdminChangeUserStatusCommand(Guid UserId, string Status) : IRequest<Result>;
