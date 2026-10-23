using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.GetSeriesById;

internal sealed class GetSeriesByIdHandler : IRequestHandler<GetSeriesByIdQuery, Result<SeriesDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISeriesRepository _repository;
	private readonly IStorageService _storageService;

	public GetSeriesByIdHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext, IStorageService storageService)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<SeriesDto>> Handle(GetSeriesByIdQuery request, CancellationToken cancellationToken)
	{
		var seriesId = SeriesId.From(request.SeriesId);

		var series = await _repository.GetByIdAsync(seriesId, cancellationToken);

		if (series is null)
			return Result.Failure<SeriesDto>(SeriesErrors.NotFound);

		if (series.SourceType == SeriesSource.Learner && series.UserId != _currentUserContext.UserId)
			return Result.Failure<SeriesDto>(UserErrors.UnauthorizedResourceAccess);

		var dto = SeriesDto.FromEntity(series);

		if (dto.ThumbnailKey != null)
		{
			var thumbnailUrl = await _storageService.CreatePresignedReadUrlAsync(dto.ThumbnailKey, cancellationToken);

			dto = dto with { ThumbnailKey = thumbnailUrl };
		}

		return Result.Success(dto);
	}
}
