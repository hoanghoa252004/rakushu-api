using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.DeleteTranscriptSegment;

public sealed record DeleteTranscriptSegmentCommand(Guid TranscriptSegmentId) : IRequest<Result>;
