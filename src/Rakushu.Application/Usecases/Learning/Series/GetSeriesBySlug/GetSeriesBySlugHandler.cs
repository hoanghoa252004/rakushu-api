using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Learning.Series.GetSeriesBySlug;

internal sealed class GetSeriesBySlugHandler : IRequestHandler<GetSeriesBySlugQuery, Result<SeriesDto>>
{
	private readonly ISeriesRepository _repository;

	public GetSeriesBySlugHandler(ISeriesRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<SeriesDto>> Handle(GetSeriesBySlugQuery request, CancellationToken cancellationToken)
	{
		var series = await _repository.GetBySlugAsync(request.Slug, cancellationToken);

		if (series is null)
			return Result.Failure<SeriesDto>(SeriesErrors.NotFound);

		return Result.Success(SeriesDto.FromEntity(series));
	}
}
