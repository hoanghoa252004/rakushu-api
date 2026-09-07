using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Admin.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDto>>;
