using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
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
	private readonly IStorageService _storageService;

	private readonly ISeriesRepository _repository;

	public GetMySeriesHandler(ISeriesRepository repository, ICurrentUserContext currentUserContext, IStorageService storageService)
	{
		_repository = repository;
		_currentUserContext = currentUserContext;
		_storageService = storageService;
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

		List<SeriesDto> result = new List<SeriesDto>();

		foreach (var dto in dtos)
		{
			if (dto.ThumbnailKey != null)
			{
				var thumbnailUrl = await _storageService.CreatePresignedReadUrlAsync(dto.ThumbnailKey, cancellationToken);

				var updatedDto = dto with { ThumbnailKey = thumbnailUrl };

				result.Add(updatedDto);
			}
			else
			{
				result.Add(dto);
			}
		}

		return Result.Success<IReadOnlyCollection<SeriesDto>>(result);
	}
}