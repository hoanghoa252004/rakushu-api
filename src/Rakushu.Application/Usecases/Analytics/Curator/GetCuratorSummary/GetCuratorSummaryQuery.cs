using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Curator.GetCuratorSummary;

public sealed record GetCuratorSummaryQuery : IRequest<Result<CuratorSummaryDto>>;
