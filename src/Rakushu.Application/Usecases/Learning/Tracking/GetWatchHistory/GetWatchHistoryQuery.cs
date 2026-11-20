using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetWatchHistory;

public sealed record GetWatchHistoryQuery : IRequest<Result<IReadOnlyList<WatchedVideoHistoryItemDto>>>;
