using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitsByVideo;

public sealed record GetVideoLearningUnitsQuery(
	Guid VideoId,
	int PageNumber,
	int PageSize,
	string? Type) : IRequest<Result<PaginatedList<LearningUnitSummaryDto>>>;