using Entity = Rakushu.Domain.Entities.Material.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetVideoById;

public sealed record GetVideoByIdQuery(Guid VideoId) : IRequest<Result<VideoDto>>;
