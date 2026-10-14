using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Video.GetVideoById;

public sealed record GetVideoByIdQuery(Guid VideoId) : IRequest<Result<VideoDetailDto>>;
