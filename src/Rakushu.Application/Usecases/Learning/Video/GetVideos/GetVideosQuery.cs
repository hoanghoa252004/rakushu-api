using Entity = Rakushu.Domain.Entities.Video.Video;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video;
using Rakushu.Application.Usecases.Learning.Video;

namespace Rakushu.Application.Usecases.Learning.Video.GetVideos;

public sealed record GetVideosQuery : IRequest<Result<IReadOnlyCollection<VideoDto>>>;
