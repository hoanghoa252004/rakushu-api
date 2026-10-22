using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Material.Transcript.GetTranscript;

public sealed record GetDetailTranscriptQuery(
	Guid VideoId,
	int PageNumber = 1,
	int PageSize = 20,
	TimeSpan? StartTime = null
) : IRequest<Result<PaginatedList<TranscriptSegmentDetailDto>>>;
