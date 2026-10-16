using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.DeleteSeries;

internal sealed class DeleteSeriesHandler : IRequestHandler<DeleteSeriesCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISeriesRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteSeriesHandler(ISeriesRepository repository, IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var seriesId = SeriesId.From(request.SeriesId);

			var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

			if (series is null)
				return Result.Failure(SeriesErrors.NotFound);

			if(series.SourceType == SeriesSource.Curated && RoleCodes.LinguisticCurator != _currentUserContext.Role)
				return Result.Failure(UserErrors.UnauthorizedResourceAccess);

			if (series.SourceType == SeriesSource.Learner && series.UserId != _currentUserContext.UserId)
				return Result.Failure(UserErrors.UnauthorizedResourceAccess);

			// Check if series has videos - if so, prevent deletion
			if (series.Videos.Any())
				return Result.Failure(SeriesErrors.HasVideos);

			// Delete series
			_repository.Delete(series);

			return Result.Success();
		});
	}
}
