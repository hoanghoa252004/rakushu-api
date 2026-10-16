using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.GetSeriesById;

internal sealed class GetSeriesByIdHandler : IRequestHandler<GetSeriesByIdQuery, Result<SeriesDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISeriesRepository _repository;

	public GetSeriesByIdHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<SeriesDto>> Handle(GetSeriesByIdQuery request, CancellationToken cancellationToken)
	{
		var seriesId = SeriesId.From(request.SeriesId);

		var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

		if (series is null)
			return Result.Failure<SeriesDto>(SeriesErrors.NotFound);

		if (series.SourceType == SeriesSource.Learner && series.UserId != _currentUserContext.UserId)
			return Result.Failure<SeriesDto>(UserErrors.UnauthorizedResourceAccess);

		return Result.Success(SeriesDto.FromEntity(series));
	}
}
