using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Series.CreateSeries;

public sealed record CreateSeriesCommand(
	string Slug,
	string Name,
	string? Description,
	int DisplayOrder) : IRequest<Result<Guid>>;
