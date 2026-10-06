using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;
using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Application.Usecases.Learning.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategorys;

public sealed record GetContentCategorysQuery(
	int PageNumber = 1,
	int PageSize = 10,
	ContentCategoryStatus? Status = null
) : IRequest<Result<PaginatedList<ContentCategoryDto>>>;
