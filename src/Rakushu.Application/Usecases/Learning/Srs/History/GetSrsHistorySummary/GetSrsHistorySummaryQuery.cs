using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.History.GetSrsHistorySummary;

public sealed record GetSrsHistorySummaryQuery : IRequest<Result<SrsHistorySummaryDto>>;
