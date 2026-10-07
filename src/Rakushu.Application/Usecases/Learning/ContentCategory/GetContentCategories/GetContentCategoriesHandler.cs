using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategories;

internal sealed class GetContentCategoriesHandler : IRequestHandler<GetContentCategoriesQuery, Result<PaginatedList<ContentCategoryDto>>>
{
	private readonly ICurrentUserContext _currentUserContext;

	private readonly IContentCategoryRepository _repository;

	public GetContentCategoriesHandler(IContentCategoryRepository repository,
		ICurrentUserContext currentUserContext)
	{
		_currentUserContext = currentUserContext;
		_repository = repository;
	}

	public async Task<Result<PaginatedList<ContentCategoryDto>>> Handle(GetContentCategoriesQuery request, CancellationToken cancellationToken)
	{
		// Authorize resource
		var isAdmin = _currentUserContext.Role == RoleCodes.SystemAdministrator;

		var list = await _repository.GetAllAsync(cancellationToken);

		var filteredByRole = !isAdmin
			? list.Where(c => c.IsActive)
			: list;

		// Calculate total count before paging
		var totalCount = filteredByRole.Count();

		// Apply paging
		var items = filteredByRole
			.OrderBy(c => c.DisplayOrder)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.Select(c => new ContentCategoryDto(
				c.Id.Value,
				c.Slug,
				c.Code,
				c.Name,
				c.JapaneseName,
				c.Description,
				c.DisplayOrder,
				c.IsActive,
				c.CreatedAt,
				c.UpdatedAt
			))
			.ToList();

		var pagedResult = PaginatedList<ContentCategoryDto>.Create(items, totalCount, request.PageNumber, request.PageSize);

		return Result.Success(pagedResult);
	}
}
