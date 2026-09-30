using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Interest.GetInterestById;

public sealed record GetInterestByIdQuery(Guid InterestId) : IRequest<Result<InterestDto>>;
