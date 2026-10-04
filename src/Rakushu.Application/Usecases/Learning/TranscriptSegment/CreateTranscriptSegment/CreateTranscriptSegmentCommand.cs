using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateTranscriptSegment;

public sealed record CreateTranscriptSegmentCommand(
	Guid transcriptId,
	string text,
	TimeSpan startTime,
	TimeSpan endTime,
	int sequence
) : IRequest<Result<Guid>>;
