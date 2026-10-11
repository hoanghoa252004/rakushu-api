using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Series;

namespace Rakushu.Application.Usecases.Learning.Series.DeleteSeries;

internal sealed class DeleteSeriesHandler : IRequestHandler<DeleteSeriesCommand, Result>
{
	private readonly ISeriesRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteSeriesHandler(ISeriesRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var seriesId = SeriesId.From(request.SeriesId);
			var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

			if (series is null)
				return Result.Failure(SeriesErrors.NotFound);

			// Check if series has videos - if so, prevent deletion
			if (series.Videos.Any())
				return Result.Failure(SeriesErrors.HasVideos);

			// Delete series
			_repository.Delete(series);

			return Result.Success();
		});
	}
}
