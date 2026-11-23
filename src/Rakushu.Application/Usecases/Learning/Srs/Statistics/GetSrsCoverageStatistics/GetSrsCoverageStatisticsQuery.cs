using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Statistics.GetSrsCoverageStatistics;

public sealed record GetSrsCoverageStatisticsQuery : IRequest<Result<SrsCoverageStatisticsDto>>;
