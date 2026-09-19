using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Feature.ChangeFeatureStatus;

internal sealed class ChangeFeatureStatusHandler : IRequestHandler<ChangeFeatureStatusCommand, Result>
{
	// DAOs
	private readonly IFeatureRepository _featureRepository;

	// UNIT OF WORK
	private readonly IUnitOfWork _unitOfWork;

	public ChangeFeatureStatusHandler(
		IFeatureRepository featureRepository,
		IUnitOfWork unitOfWork
		)
	{
		_featureRepository = featureRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangeFeatureStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var featureId = FeatureId.From(request.FeatureId);

			var feature = await _featureRepository.GetByIdAsync(featureId, cancellationToken);

			if (feature == null)
			{
				return Result.Failure(FeatureErrors.NotFound);
			}

			if (!Enum.TryParse<FeatureStatus>(request.Status, true, out var status))
			{
				return Result.Failure<Guid>(FeatureErrors.InvalidStatus);
			}

			return feature.ChangeStatus(status);
		}, cancellationToken);
	}
}
