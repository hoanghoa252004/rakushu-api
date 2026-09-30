using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.TranscriptSegment.GetTranscriptSegmentById;

public sealed record GetTranscriptSegmentByIdQuery(Guid TranscriptSegmentId) : IRequest<Result<TranscriptSegmentDto>>;
