using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Interest.GetInterests;

public sealed record GetInterestsQuery : IRequest<Result<IReadOnlyCollection<InterestDto>>>;
