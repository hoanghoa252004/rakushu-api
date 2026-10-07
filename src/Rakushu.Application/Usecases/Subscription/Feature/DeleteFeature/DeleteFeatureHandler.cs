using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Subscription.Feature.DeleteFeature;

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
			var id = FeatureId.From(request.FeatureId);

			var feature = await _featureRepository.GetByIdAsync(id, cancellationToken);

			if (feature is null)
			{
				return Result.Failure(FeatureErrors.NotFound);
			}

			if (feature.Entitlements.Any() || feature.SubscriptionUsages.Any())
			{
				return Result.Failure(FeatureErrors.CannotDeleteFeatureHasBeenAtachedToAPlan);
			}

			_featureRepository.Delete(feature);

			return Result.Success();
		}, cancellationToken);
	}
}
