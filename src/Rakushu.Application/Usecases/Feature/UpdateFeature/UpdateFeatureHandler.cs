using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Feature.UpdateFeature;

internal sealed class UpdateFeatureHandler : IRequestHandler<UpdateFeatureCommand, Result>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	// SERVICES
	private readonly ISystemClock _systemClock;

	public UpdateFeatureHandler(
		IFeatureRepository featureRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_featureRepository = featureRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
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

			var result = feature.Update(request.Name, request.Description, _systemClock.UtcNow);

			if (result.IsFailure)
			{
				return result;
			}

			return Result.Success();
		}, cancellationToken);
	}
}
