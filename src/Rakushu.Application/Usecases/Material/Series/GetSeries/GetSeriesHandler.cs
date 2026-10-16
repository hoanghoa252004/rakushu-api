using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.GetAllSeries;

internal sealed class GetSeriesHandler : IRequestHandler<GetSeriesQuery, Result<PaginatedList<SeriesDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;

	private readonly ISeriesRepository _repository;

	public GetSeriesHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<PaginatedList<SeriesDto>>> Handle(GetSeriesQuery request, CancellationToken cancellationToken)
	{
		var series = (await _repository.GetAllAsync(cancellationToken)).Where(s => s.SourceType == SeriesSource.Curated);

		if(_currentUserContext.Role == RoleCodes.Learner)
		{
			series = series.Where(s => s.IsActive).ToList();
		}

		var dtos = series
			.Select(SeriesDto.FromEntity)
			.OrderBy(s => s.DisplayOrder)
			.ToList();

		var paginatedList = PaginatedList<SeriesDto>.Create(
			dtos,
			dtos.Count,
			request.PageNumber,
			request.PageSize);

		return Result.Success(paginatedList);
	}
}
