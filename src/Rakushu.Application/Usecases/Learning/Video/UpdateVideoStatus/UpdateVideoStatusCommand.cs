using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Learning.Video.UpdateVideoStatus;

public sealed record UpdateVideoStatusCommand(
	Guid VideoId,
	VideoStatus NewStatus
) : IRequest<Result>;
