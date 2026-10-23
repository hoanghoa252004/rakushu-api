using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.UpdateSegment;

public sealed record UpdateTranscriptSegmentCommand(
	Guid VideoId,
	Guid SegmentId,
	Guid SpeakerId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	SubtitleInput Subtitle,
	IReadOnlyList<TranscriptItemInput> Items,
	int? Sequence = null
) : IRequest<Result>;
