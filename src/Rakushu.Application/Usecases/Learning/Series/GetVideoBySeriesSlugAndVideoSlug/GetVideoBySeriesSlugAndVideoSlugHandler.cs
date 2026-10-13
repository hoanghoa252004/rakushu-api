using MediatR;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Series;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Series.GetVideoBySeriesSlugAndVideoSlug;

internal sealed class GetVideoBySeriesSlugAndVideoSlugHandler : IRequestHandler<GetVideoBySeriesSlugAndVideoSlugQuery, Result<VideoDto>>
{
	private readonly ISeriesRepository _seriesRepository;

	public GetVideoBySeriesSlugAndVideoSlugHandler(ISeriesRepository seriesRepository)
	{
		_seriesRepository = seriesRepository;
	}

	public async Task<Result<VideoDto>> Handle(
		GetVideoBySeriesSlugAndVideoSlugQuery request,
		CancellationToken cancellationToken)
	{
		// Get series by slug
		var series = await _seriesRepository.GetBySlugAsync(request.SeriesSlug, cancellationToken);
		if (series is null)
			return Result.Failure<VideoDto>(SeriesErrors.NotFound);

		// Find video in series by slug
		var video = series.Videos.FirstOrDefault(v => v.Slug == request.VideoSlug);
		if (video is null)
			return Result.Failure<VideoDto>(VideoErrors.NotFound);

		return Result.Success(VideoDto.FromEntity(video));
	}
}
