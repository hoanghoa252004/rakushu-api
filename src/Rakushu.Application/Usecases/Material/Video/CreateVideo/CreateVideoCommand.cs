using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Video.CreateVideo;

public sealed record CreateVideoCommand(
	string Slug,
	string Title,
	string? Description,
	TimeSpan Duration,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder,
	string SourceType,
	string? SourceUrl,
	string StorageKeyOfVideo,
	string StorageKeyOfThumbnail
) : IRequest<Result<Guid>>;

