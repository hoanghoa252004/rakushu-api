using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.UpdateInterest;

public sealed record UpdateInterestCommand(
	Guid InterestId,
	Guid profileId,
	Guid contentCategoryId,
	int priority
) : IRequest<Result>;
