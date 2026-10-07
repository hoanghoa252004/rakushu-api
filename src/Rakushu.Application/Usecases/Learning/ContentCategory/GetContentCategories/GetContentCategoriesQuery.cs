using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategories;

public sealed record GetContentCategoriesQuery(
	int PageNumber = 1,
	int PageSize = 10
) : IRequest<Result<PaginatedList<ContentCategoryDto>>>;
