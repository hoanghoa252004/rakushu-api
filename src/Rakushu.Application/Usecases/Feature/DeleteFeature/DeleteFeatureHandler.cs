using MediatR;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Feature.DeleteFeature;

internal sealed class DeleteFeatureHandler : IRequestHandler<DeleteFeatureCommand, Result>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public DeleteFeatureHandler(
		IFeatureRepository featureRepository,
		IUnitOfWork unitOfWork)
	{
		_featureRepository = featureRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var feature = await _featureRepository.GetByIdAsync(
				FeatureId.From(request.FeatureId),
				cancellationToken);

			if (feature is null)
			{
				return Result.Failure(FeatureErrors.NotFound);
			}

			if(feature.Status != FeatureStatus.Draft) // if not draft then continue to check
			{
				// Check if feature has subscription usage
				if (feature.SubscriptionUsages.Any() == true)
				{
					return Result.Failure(FeatureErrors.CannotDeleteFeatureWithSubscriptionUsage);
				}

				// Check if feature has attached to any plan
				if (feature.PlanEntitlements.Any() == true)
				{
					return Result.Failure(FeatureErrors.CannotDeleteFeatureHasBeenAtachedToAPlan);
				}
			}

			_featureRepository.Delete(feature);

			return Result.Success();
		}, cancellationToken);
	}
}
