using MediatR;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryBySlug;

internal sealed class GetContentCategoryBySlugHandler : IRequestHandler<GetContentCategoryBySlugQuery, Result<ContentCategoryDto>>
{
	private readonly IContentCategoryRepository _repository;

	public GetContentCategoryBySlugHandler(IContentCategoryRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<ContentCategoryDto>> Handle(GetContentCategoryBySlugQuery request, CancellationToken cancellationToken)
	{
		var category = await _repository.GetBySlugAsync(request.Slug, cancellationToken);

		if(category is null)
			return Result.Failure<ContentCategoryDto>(ContentCategoryErrors.NotFound);

		if(category.IsActive == false)
			return Result.Failure<ContentCategoryDto>(UserErrors.UnauthorizedResourceAccess);

		return Result.Success(new ContentCategoryDto(
			category.Id.Value,
			category.Slug,
			category.Code,
			category.Name,
			category.JapaneseName,
			category.Description,
			category.DisplayOrder,
			category.IsActive,
			category.CreatedAt,
			category.UpdatedAt
		));
	}
}