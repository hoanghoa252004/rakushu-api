using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsDashboardMetrics;

public sealed record GetSrsDashboardMetricsQuery : IRequest<Result<SrsDashboardMetricsDto>>;
