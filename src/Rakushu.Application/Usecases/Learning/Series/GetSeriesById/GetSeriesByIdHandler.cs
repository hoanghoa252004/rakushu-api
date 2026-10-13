using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Learning.Series.GetSeriesById;

internal sealed class GetSeriesByIdHandler : IRequestHandler<GetSeriesByIdQuery, Result<SeriesDto>>
{
	private readonly ISeriesRepository _repository;

	public GetSeriesByIdHandler(ISeriesRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<SeriesDto>> Handle(GetSeriesByIdQuery request, CancellationToken cancellationToken)
	{
		var seriesId = SeriesId.From(request.SeriesId);
		var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

		if (series is null)
			return Result.Failure<SeriesDto>(SeriesErrors.NotFound);

		return Result.Success(SeriesDto.FromEntity(series));
	}
}
