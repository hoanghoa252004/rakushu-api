using MediatR;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.TranscriptSegment.GetSegmentById;

public sealed record GetTranscriptSegmentByIdQuery(
	Guid VideoId,
	Guid SegmentId
) : IRequest<Result<TranscriptSegmentDetailDto>>;
