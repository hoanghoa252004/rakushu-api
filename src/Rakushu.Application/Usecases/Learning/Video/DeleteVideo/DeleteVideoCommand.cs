using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Video.DeleteVideo;

public sealed record DeleteVideoCommand(Guid VideoId) : IRequest<Result>;
