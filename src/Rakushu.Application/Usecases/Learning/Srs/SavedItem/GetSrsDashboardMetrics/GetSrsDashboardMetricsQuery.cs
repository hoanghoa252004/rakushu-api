using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

public sealed record GetSrsDashboardMetricsQuery(string? TimeZone = null) : IRequest<Result<SrsDashboardMetricsDto>>;
