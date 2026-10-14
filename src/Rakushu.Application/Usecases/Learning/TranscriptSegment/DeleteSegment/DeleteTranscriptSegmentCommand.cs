using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.DeleteSegment;

public sealed record DeleteTranscriptSegmentCommand(
	Guid VideoId,
	Guid SegmentId
) : IRequest<Result>;
