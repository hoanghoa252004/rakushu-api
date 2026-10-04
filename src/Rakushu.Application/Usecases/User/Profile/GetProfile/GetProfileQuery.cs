using MediatR;
using Rakushu.Application.Usecases.User.User.GetUserById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.User.Profile.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<UserDetailDto>>;
