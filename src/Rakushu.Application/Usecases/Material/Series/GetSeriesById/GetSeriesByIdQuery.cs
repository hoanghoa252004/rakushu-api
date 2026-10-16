using MediatR;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.GetSeriesById;

public sealed record GetSeriesByIdQuery(Guid SeriesId) : IRequest<Result<SeriesDto>>;
