using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.Transcript.GetLearningTranscript;

public sealed record GetLearningTranscriptQuery(
	Guid VideoId,
	int PageNumber = 1,
	int PageSize = 20,
	TimeSpan? StartTime = null
) : IRequest<Result<PaginatedList<TranscriptSegmentBasicDto>>>;