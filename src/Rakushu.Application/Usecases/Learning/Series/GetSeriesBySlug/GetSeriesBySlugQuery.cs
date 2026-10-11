using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Series.GetSeriesBySlug;

public sealed record GetSeriesBySlugQuery(string Slug) : IRequest<Result<SeriesDto>>;
