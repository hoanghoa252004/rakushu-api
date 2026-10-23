using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.CreateSeries;

internal sealed class CreateSeriesHandler : IRequestHandler<CreateSeriesCommand, Result<Guid>>
{
	private readonly ICurrentUserContext _currentUserContext;

	private readonly ISeriesRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateSeriesHandler(ISeriesRepository repository, IUnitOfWork unitOfWork,
		ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<Guid>> Handle(CreateSeriesCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Determine SeriesSource based on user role
			var sourceType = _currentUserContext.Role == RoleCodes.LinguisticCurator
				? SeriesSource.Curated
				: SeriesSource.Learner;

			// Create new series using the domain factory
			var series = Domain.Entities.Material.Series.Series.Create(
				request.Slug,
				request.Name,
				request.JapaneseName,
				request.DisplayOrder,
				sourceType,
				request.IsActive,
				sourceType == SeriesSource.Learner ? _currentUserContext.UserId : null,
				request.Description,
				request.ThumbnailKey);

			// Add to repository
			_repository.Add(series);

			return Result.Success(series.Id.Value);
		});
	}
}
