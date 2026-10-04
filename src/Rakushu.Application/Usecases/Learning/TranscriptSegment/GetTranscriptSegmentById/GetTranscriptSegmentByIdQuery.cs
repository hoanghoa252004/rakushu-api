using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Learning.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.GetTranscriptSegmentById;

public sealed record GetTranscriptSegmentByIdQuery(Guid TranscriptSegmentId) : IRequest<Result<TranscriptSegmentDto>>;
