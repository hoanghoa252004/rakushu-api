using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateSegment;

public sealed record UpdateTranscriptSegmentCommand(
	Guid VideoId,
	Guid SegmentId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null
) : IRequest<Result<TranscriptSegmentDetailDto>>;
