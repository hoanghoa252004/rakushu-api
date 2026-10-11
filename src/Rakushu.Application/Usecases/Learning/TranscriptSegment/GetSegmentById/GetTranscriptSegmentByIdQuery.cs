using MediatR;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.GetSegmentById;

public sealed record GetTranscriptSegmentByIdQuery(
	Guid VideoId,
	Guid SegmentId
) : IRequest<Result<TranscriptSegmentDetailDto>>;
