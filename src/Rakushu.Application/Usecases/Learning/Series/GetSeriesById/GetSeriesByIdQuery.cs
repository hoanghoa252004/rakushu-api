using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Series.GetSeriesById;

public sealed record GetSeriesByIdQuery(Guid SeriesId) : IRequest<Result<SeriesDto>>;
