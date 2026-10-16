using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Series.GetAllSeries;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Material.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Series.GetMySeries;

internal sealed class GetMySeriesHandler : IRequestHandler<GetMySeriesQuery, Result<IReadOnlyCollection<SeriesDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;

	private readonly ISeriesRepository _repository;

	public GetMySeriesHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<IReadOnlyCollection<SeriesDto>>> Handle(GetMySeriesQuery request, CancellationToken cancellationToken)
	{
		var series = (await _repository.GetAllAsync(cancellationToken))
			.Where(s => s.SourceType == SeriesSource.Learner
				&& s.UserId == _currentUserContext.UserId);

		var dtos = series
			.Select(SeriesDto.FromEntity)
			.OrderBy(s => s.DisplayOrder)
			.ToList();

		return Result.Success<IReadOnlyCollection<SeriesDto>>(dtos);
	}
}