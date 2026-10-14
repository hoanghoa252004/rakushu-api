using Entity = Rakushu.Domain.Entities.Material.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Video.UpdateVideo;

public sealed record UpdateVideoCommand(
	Guid VideoId,
	string Title,
	string? Description,
	Guid ContentCategoryId,
	Guid? SeriesId,
	int SortOrder
) : IRequest<Result>;
