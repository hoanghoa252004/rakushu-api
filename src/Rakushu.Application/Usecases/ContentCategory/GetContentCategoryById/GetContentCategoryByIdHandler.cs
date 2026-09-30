using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.ContentCategory.GetContentCategoryById;

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

		return Result.Success(ContentCategoryDto.FromEntity(category));
	}
}