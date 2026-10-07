using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.DeleteContentCategory;

internal sealed class DeleteContentCategoryHandler : IRequestHandler<DeleteContentCategoryCommand, Result>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteContentCategoryHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteContentCategoryCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = ContentCategoryId.From(request.ContentCategoryId);

			var category = await _repository.GetByIdAsync(id, cancellationToken);

			if (category is null)
				return Result.Failure(ContentCategoryErrors.NotFound);

			if(category.Videos.Any() || category.Series.Any() || category.ContentProcessingPolicies.Any() || category.Interests.Any())
				return Result.Failure(ContentCategoryErrors.IsInUse);

			_repository.Delete(category);

			return Result.Success();
		}, cancellationToken);
	}
}
