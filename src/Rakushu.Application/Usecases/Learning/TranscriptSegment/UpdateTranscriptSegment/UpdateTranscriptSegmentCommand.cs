using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.UpdateTranscriptSegment;

public sealed record UpdateTranscriptSegmentCommand(
	Guid TranscriptSegmentId,
	Guid transcriptId,
	string text,
	TimeSpan startTime,
	TimeSpan endTime,
	int sequence
) : IRequest<Result>;
