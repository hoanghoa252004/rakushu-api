using Entity = Rakushu.Domain.Entities.User.Profile.Profile;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProfileAdmin.GetProfileById;

public sealed record GetProfileByIdQuery(Guid ProfileId) : IRequest<Result<ProfileDto>>;
