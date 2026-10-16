using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Knowledge.ContentCategory.GetContentCategories;

public sealed record GetContentCategoriesQuery(
	int PageNumber = 1,
	int PageSize = 10
) : IRequest<Result<PaginatedList<ContentCategoryDto>>>;
