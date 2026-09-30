using Entity = Rakushu.Domain.Entities.User.Profile.Profile;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Application.Usecases.ProfileAdmin.DeleteProfile;

public sealed record DeleteProfileCommand(Guid ProfileId) : IRequest<Result>;
