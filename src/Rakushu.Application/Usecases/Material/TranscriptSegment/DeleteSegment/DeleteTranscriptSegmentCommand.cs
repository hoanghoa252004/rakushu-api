using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.DeleteSegment;

public sealed record DeleteTranscriptSegmentCommand(
	Guid VideoId,
	Guid SegmentId
) : IRequest<Result>;
