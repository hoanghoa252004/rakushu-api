using MediatR;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Series.GetSeriesBySlug;

public sealed record GetSeriesBySlugQuery(string Slug) : IRequest<Result<SeriesDto>>;
