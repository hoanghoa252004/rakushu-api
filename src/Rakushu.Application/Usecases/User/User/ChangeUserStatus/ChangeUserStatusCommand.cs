using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.User.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(
	Guid UserId,
	string Status
) : IRequest<Result>;
