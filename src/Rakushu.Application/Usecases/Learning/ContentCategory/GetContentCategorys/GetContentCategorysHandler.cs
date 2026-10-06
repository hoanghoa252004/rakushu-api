using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.ContentCategory;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategorys;

internal sealed class GetContentCategorysHandler : IRequestHandler<GetContentCategorysQuery, Result<PaginatedList<ContentCategoryDto>>>
{
	private readonly IContentCategoryRepository _repository;

	public GetContentCategorysHandler(IContentCategoryRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<PaginatedList<ContentCategoryDto>>> Handle(GetContentCategorysQuery request, CancellationToken cancellationToken)
	{
		// Validate paging parameters
		var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
		var pageSize = request.PageSize < 1 || request.PageSize > 100 ? 10 : request.PageSize;

		IEnumerable<Entity> query;

		// Apply status filter if provided (admin can filter by status)
		if (request.Status.HasValue)
		{
			query = await _repository.GetByStatusAsync(request.Status.Value, cancellationToken);
		}
		else
		{
			// Default: get all active categories for learners
			query = await _repository.GetActiveAsync(cancellationToken);
		}

		// Calculate total count before paging
		var totalCount = query.Count();

		// Apply paging
		var items = query
			.OrderBy(c => c.DisplayOrder)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(ContentCategoryDto.FromEntity)
			.ToList();

		var pagedResult = PaginatedList<ContentCategoryDto>.Create(items, totalCount, pageNumber, pageSize);
		return Result.Success(pagedResult);
	}
}
