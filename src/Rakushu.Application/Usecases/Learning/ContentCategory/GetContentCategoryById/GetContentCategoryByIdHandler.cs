using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;

internal sealed class GetContentCategoryByIdHandler : IRequestHandler<GetContentCategoryByIdQuery, Result<ContentCategoryDto>>
{
	private readonly IContentCategoryRepository _repository;

	public GetContentCategoryByIdHandler(IContentCategoryRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<ContentCategoryDto>> Handle(GetContentCategoryByIdQuery request, CancellationToken cancellationToken)
	{
		var id = ContentCategoryId.From(request.ContentCategoryId);

		var category = await _repository.GetByIdAsync(id, cancellationToken);

		if (category is null)
			return Result.Failure<ContentCategoryDto>(ContentCategoryErrors.NotFound);

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
