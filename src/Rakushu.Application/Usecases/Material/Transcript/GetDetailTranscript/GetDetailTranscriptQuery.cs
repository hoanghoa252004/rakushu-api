using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Transcript.GetDetailTranscript;

public sealed record GetDetailTranscriptQuery(
	Guid VideoId,
	int PageNumber = 1,
	int PageSize = 20,
	TimeSpan? StartTime = null
) : IRequest<Result<PaginatedList<TranscriptSegmentDetailDto>>>;
