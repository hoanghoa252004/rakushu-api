using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.CreateSegment;

public sealed record CreateTranscriptSegmentCommand(
	Guid VideoId,
	Guid SpeakerId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null
) : IRequest<Result<Guid>>;
