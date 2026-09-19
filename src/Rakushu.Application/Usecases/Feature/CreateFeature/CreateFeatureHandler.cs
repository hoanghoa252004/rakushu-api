using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using Rakushu.Domain.Entities.Feature.ObjectValues;

namespace Rakushu.Application.Usecases.Feature.CreateFeature;

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
			// Validate code format
			var codeResult = FeatureCode.Create(request.Code);
			if (codeResult.IsFailure)
			{
				return Result.Failure<Guid>(codeResult.Error);
			}

			var code = codeResult.Value;

			// Check if feature with same code already exists
			var existingFeature = await _featureRepository.GetByCodeAsync(code, cancellationToken);
			if (existingFeature is not null)
			{
				return Result.Failure<Guid>(FeatureErrors.DuplicateCode);
			}

			// Create feature
			var now = _systemClock.UtcNow;

			var initialStatus = FeatureStatus.Draft;

			var featureResult = Domain.Entities.Feature.Feature.Create(
				code,
				request.Name,
				initialStatus,
				now,
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
