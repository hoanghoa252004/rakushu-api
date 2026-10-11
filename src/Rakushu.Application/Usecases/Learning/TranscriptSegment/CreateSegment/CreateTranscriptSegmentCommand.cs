using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.CreateSegment;

public sealed record CreateTranscriptSegmentCommand(
	Guid VideoId,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	int? Sequence = null
) : IRequest<Result<TranscriptSegmentDetailDto>>;
