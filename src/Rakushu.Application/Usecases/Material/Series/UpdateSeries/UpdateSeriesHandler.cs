using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.UpdateSeries;

internal sealed class UpdateSeriesHandler : IRequestHandler<UpdateSeriesCommand, Result>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISeriesRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateSeriesHandler(ISeriesRepository repository, IUnitOfWork unitOfWork, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result> Handle(UpdateSeriesCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var seriesId = SeriesId.From(request.SeriesId);
			var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

			if (series is null)
				return Result.Failure(SeriesErrors.NotFound);

			if (series.SourceType == SeriesSource.Learner && series.UserId != _currentUserContext.UserId)
				return Result.Failure<SeriesDto>(UserErrors.UnauthorizedResourceAccess);

			// Update series (note: SourceType cannot be updated)
			series.Update(
				request.Name,
				request.Description,
				request.DisplayOrder,
				request.IsActive);

			return Result.Success();
		});
	}
}
