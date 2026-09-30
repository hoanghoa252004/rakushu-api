using Entity = Rakushu.Domain.Entities.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;

namespace Rakushu.Application.Usecases.Video.GetVideoById;

public sealed record GetVideoByIdQuery(Guid VideoId) : IRequest<Result<VideoDto>>;
