using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegmentSpeaker;

public sealed record UpdateTranscriptSegmentSpeakerCommand(
	Guid VideoId,
	Guid SegmentId,
	Guid? SpeakerId = null,
	Guid? TranscriptSpeakerId = null
) : IRequest<Result>;
