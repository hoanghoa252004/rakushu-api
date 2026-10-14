using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.TranscriptSegment.GetSegments;

public sealed record GetTranscriptSegmentsQuery(
	Guid VideoId,
	int PageNumber = 1,
	int PageSize = 20,
	TimeSpan? StartTime = null
) : IRequest<Result<PaginatedList<TranscriptSegmentDetailDto>>>;
