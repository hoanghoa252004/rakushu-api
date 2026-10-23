using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.CreateSeries;

public sealed record CreateSeriesCommand(
	string Slug,
	string Name,
	string JapaneseName,
	string? ThumbnailKey,
	string? Description,
	int DisplayOrder,
	bool IsActive) : IRequest<Result<Guid>>;
