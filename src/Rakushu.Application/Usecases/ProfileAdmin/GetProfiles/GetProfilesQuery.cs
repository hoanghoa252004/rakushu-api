using Entity = Rakushu.Domain.Entities.User.Profile.Profile;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.ProfileAdmin.GetProfiles;

public sealed record GetProfilesQuery : IRequest<Result<IReadOnlyCollection<ProfileDto>>>;
