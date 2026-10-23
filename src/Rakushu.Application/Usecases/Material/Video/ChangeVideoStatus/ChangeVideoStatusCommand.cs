using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Application.Usecases.Material.Video.ChangeVideoStatus;

public sealed record ChangeVideoStatusCommand(
	Guid VideoId,
	VideoStatus NewStatus
) : IRequest<Result>;
