using MediatR;
using Rakushu.Application.Usecases.Users.GetUserById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Profile.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<UserDto>>;
