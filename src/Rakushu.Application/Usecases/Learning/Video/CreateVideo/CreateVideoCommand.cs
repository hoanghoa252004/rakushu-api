using Entity = Rakushu.Domain.Entities.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.CreateVideo;

public sealed record CreateVideoCommand(
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder,
	VideoSource SourceType,
	string? SourceUrl,
	string StorageKeyOfVideo
) : IRequest<Result<Guid>>;

