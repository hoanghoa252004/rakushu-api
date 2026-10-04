using MediatR;
using Rakushu.Application.Usecases.Learning.ContentCategory;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategorys;

internal sealed class GetContentCategorysHandler : IRequestHandler<GetContentCategorysQuery, Result<IReadOnlyCollection<ContentCategoryDto>>>
{
	private readonly IContentCategoryRepository _repository;

	public GetContentCategorysHandler(IContentCategoryRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<ContentCategoryDto>>> Handle(GetContentCategorysQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<ContentCategoryDto>>(list.Select(ContentCategoryDto.FromEntity).ToArray());
	}
}