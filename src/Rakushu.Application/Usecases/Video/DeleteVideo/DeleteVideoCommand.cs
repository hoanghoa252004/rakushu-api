using Entity = Rakushu.Domain.Entities.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Video.DeleteVideo;

public sealed record DeleteVideoCommand(Guid VideoId) : IRequest<Result>;
