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

			var now = DateTimeOffset.UtcNow;
			ContentCategoryId? parentId = request.parentId.HasValue ? ContentCategoryId.From(request.parentId.Value) : null;

			var updateResult = category.Update(
				request.slug,
				request.code,
				request.name,
				request.level,
				request.displayOrder,
				request.isActive,
				now,
				parentId,
				request.description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}