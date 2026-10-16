using MediatR;
using Rakushu.Application.Usecases.Material.Video;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Video.GetVideoById;

public sealed record GetVideoByIdQuery(Guid VideoId) : IRequest<Result<VideoDetailDto>>;
