using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Series;

namespace Rakushu.Application.Usecases.Material.Series.GetSeriesBySlug;

internal sealed class GetSeriesBySlugHandler : IRequestHandler<GetSeriesBySlugQuery, Result<SeriesDto>>
{
	private readonly ICurrentUserContext _currentUserContext;
	private readonly IStorageService _storageService;
	private readonly ISeriesRepository _repository;

	public GetSeriesBySlugHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext, IStorageService storageService)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
	}

	public async Task<Result<SeriesDto>> Handle(GetSeriesBySlugQuery request, CancellationToken cancellationToken)
	{
		var series = await _repository.GetBySlugAsync(request.Slug, cancellationToken);

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
