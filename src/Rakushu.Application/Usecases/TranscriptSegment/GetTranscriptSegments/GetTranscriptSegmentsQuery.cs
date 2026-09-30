using Entity = Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment.TranscriptSegment;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.TranscriptSegment.GetTranscriptSegments;

public sealed record GetTranscriptSegmentsQuery : IRequest<Result<IReadOnlyCollection<TranscriptSegmentDto>>>;
