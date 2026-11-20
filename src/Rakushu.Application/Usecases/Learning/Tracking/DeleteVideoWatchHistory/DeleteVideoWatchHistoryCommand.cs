using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.DeleteVideoWatchHistory;

public sealed record DeleteVideoWatchHistoryCommand(Guid VideoId) : IRequest<Result>;
