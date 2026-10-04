using Entity = Rakushu.Domain.Entities.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Learning.Video.CreateVideo;

public sealed record CreateVideoCommand(
	string slug,
	string title,
	string? description,
	TimeSpan duration,
	Guid contentCategoryId,
	Guid seriesId,
	int sortOrder,
	VideoSource sourceType,
	string? sourceUrl,
	VideoStatus status,
	Guid createdBy
) : IRequest<Result<Guid>>;
