using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Transcript.GetTranscript;

public sealed record GetTranscriptQuery(
	Guid VideoId,
	int PageNumber = 1,
	int PageSize = 20,
	TimeSpan? StartTime = null
) : IRequest<Result<TranscriptDetailDto>>;
