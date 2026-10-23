using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.UpdateSeries;

public sealed record UpdateSeriesCommand(
	Guid SeriesId,
	string Name,
	string JapaneseName,
	string? ThumbnailKey,
	string? Description,
	int DisplayOrder,
	bool IsActive) : IRequest<Result>;
