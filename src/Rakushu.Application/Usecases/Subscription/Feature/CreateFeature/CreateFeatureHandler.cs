using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Subscription.Feature.CreateFeature;

internal sealed class CreateFeatureHandler : IRequestHandler<CreateFeatureCommand, Result<Guid>>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;
	private readonly ISystemClock _systemClock;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public CreateFeatureHandler(
		IFeatureRepository featureRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_featureRepository = featureRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Check if feature with same code already exists
			var existingFeature = await _featureRepository.GetByCodeAsync(request.Code, cancellationToken);

			if (existingFeature is not null)
			{
				return Result.Failure<Guid>(FeatureErrors.DuplicateCode);
			}

			// Create feature
			var now = _systemClock.UtcNow;

			var featureResult = Domain.Entities.Feature.Feature.Create(
				request.Code,
				request.Name,
				request.IsActive,
				now,
				request.Description);

			if (featureResult.IsFailure)
			{
				return Result.Failure<Guid>(featureResult.Error);
			}

			var feature = featureResult.Value;

			_featureRepository.Add(feature);

			return Result.Success(feature.Id.Value);
		}, cancellationToken);
	}
}
