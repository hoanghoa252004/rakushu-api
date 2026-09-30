using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest.CreateInterest;

public sealed record CreateInterestCommand(
	Guid profileId,
	Guid contentCategoryId,
	int priority
) : IRequest<Result<Guid>>;
