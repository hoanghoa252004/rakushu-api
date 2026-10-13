using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Learning.Series.GetAllSeries;

internal sealed class GetAllSeriesHandler : IRequestHandler<GetAllSeriesQuery, Result<PaginatedList<SeriesDto>>>
{
	private readonly ISeriesRepository _repository;

	public GetAllSeriesHandler(ISeriesRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<PaginatedList<SeriesDto>>> Handle(GetAllSeriesQuery request, CancellationToken cancellationToken)
	{
		var series = await _repository.GetAllAsync(cancellationToken);

		var dtos = series
			.Select(SeriesDto.FromEntity)
			.ToList();

		var paginatedList = PaginatedList<SeriesDto>.Create(
			dtos,
			dtos.Count,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
