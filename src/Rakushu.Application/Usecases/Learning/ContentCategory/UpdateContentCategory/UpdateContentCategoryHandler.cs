using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;

internal sealed class UpdateContentCategoryHandler : IRequestHandler<UpdateContentCategoryCommand, Result>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateContentCategoryHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateContentCategoryCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var category = await _repository.GetByIdAsync(ContentCategoryId.From(request.ContentCategoryId), cancellationToken);
			if (category is null)
				return Result.Failure(ContentCategoryErrors.NotFound);

			// Validate parent if provided and different from current
			if (request.parentId.HasValue)
			{
				var parent = await _repository.GetByIdAsync(ContentCategoryId.From(request.parentId.Value), cancellationToken);
				if (parent is null)
					return Result.Failure(ContentCategoryErrors.ParentNotFound);

				// Validate parent level is less than current level
				if (parent.Level >= request.level)
					return Result.Failure(ContentCategoryErrors.InvalidParentLevel);
			}

			// Validate display order is unique within the level (exclude current category)
			if (await _repository.ExistsDisplayOrderInLevelAsync(
				request.level, 
				request.displayOrder, 
				ContentCategoryId.From(request.ContentCategoryId), 
				cancellationToken))
				return Result.Failure(ContentCategoryErrors.DuplicateDisplayOrder);

			var now = DateTimeOffset.UtcNow;
			ContentCategoryId? parentId = request.parentId.HasValue ? ContentCategoryId.From(request.parentId.Value) : null;

			var updateResult = category.Update(
				request.slug,
				request.code,
				request.name,
				request.japaneseName,
				request.level,
				request.displayOrder,
				request.status,
				now,
				parentId,
				request.description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}
