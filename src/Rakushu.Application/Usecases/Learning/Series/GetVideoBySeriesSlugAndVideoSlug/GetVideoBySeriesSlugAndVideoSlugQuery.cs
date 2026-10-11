using MediatR;
using Rakushu.Application.Usecases.Learning.Video;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Series.GetVideoBySeriesSlugAndVideoSlug;

public sealed record GetVideoBySeriesSlugAndVideoSlugQuery(
	string SeriesSlug,
	string VideoSlug
) : IRequest<Result<VideoDto>>;
