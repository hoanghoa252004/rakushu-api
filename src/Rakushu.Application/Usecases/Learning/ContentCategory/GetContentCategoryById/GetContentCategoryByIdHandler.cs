using MediatR;
using Rakushu.Application.Usecases.Learning.ContentCategory;
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
		var category = await _repository.GetByIdAsync(ContentCategoryId.From(request.ContentCategoryId), cancellationToken);
		if (category is null)
			return Result.Failure<ContentCategoryDto>(ContentCategoryErrors.NotFound);

		// Learners can only access active categories
		// Note: Authorization should be enforced at the endpoint level via policies
		// Here we just ensure the category exists

		return Result.Success(ContentCategoryDto.FromEntity(category));
	}
}
