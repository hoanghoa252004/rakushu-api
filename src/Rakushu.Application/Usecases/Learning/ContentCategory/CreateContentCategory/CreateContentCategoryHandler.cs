using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;

internal sealed class CreateContentCategoryHandler : IRequestHandler<CreateContentCategoryCommand, Result<Guid>>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateContentCategoryHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateContentCategoryCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Validate parent if provided
			if (request.parentId.HasValue)
			{
				var parent = await _repository.GetByIdAsync(ContentCategoryId.From(request.parentId.Value), cancellationToken);
				if (parent is null)
					return Result.Failure<Guid>(ContentCategoryErrors.ParentNotFound);

				// Validate parent level is less than current level
				if (parent.Level >= request.level)
					return Result.Failure<Guid>(ContentCategoryErrors.InvalidParentLevel);
			}

			// Validate display order is unique within the level
			if (await _repository.ExistsDisplayOrderInLevelAsync(request.level, request.displayOrder, null, cancellationToken))
				return Result.Failure<Guid>(ContentCategoryErrors.DuplicateDisplayOrder);

			var now = DateTimeOffset.UtcNow;
			ContentCategoryId? parentId = request.parentId.HasValue ? ContentCategoryId.From(request.parentId.Value) : null;

			var categoryResult = Domain.Entities.ContentCategory.ContentCategory.Create(
				request.slug,
				request.code,
				request.name,
				request.japaneseName,
				request.level,
				request.displayOrder,
				request.status,
				now,
				now,
				parentId,
				request.description);

			if (categoryResult.IsFailure)
				return Result.Failure<Guid>(categoryResult.Error);

			_repository.Add(categoryResult.Value);
			return Result.Success(categoryResult.Value.Id.Value);
		}, cancellationToken);
	}
}
