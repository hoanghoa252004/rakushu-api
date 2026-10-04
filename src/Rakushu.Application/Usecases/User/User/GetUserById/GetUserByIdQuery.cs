using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.User.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserDetailDto>>;
