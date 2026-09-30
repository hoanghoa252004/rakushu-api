using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.DeleteInterest;

public sealed record DeleteInterestCommand(Guid InterestId) : IRequest<Result>;
