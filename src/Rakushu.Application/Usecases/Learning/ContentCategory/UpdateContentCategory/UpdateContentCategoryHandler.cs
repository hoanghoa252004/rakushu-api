using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;

internal sealed class UpdateContentCategoryHandler : IRequestHandler<UpdateContentCategoryCommand, Result>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;
	public UpdateContentCategoryHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork,
		ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result> Handle(UpdateContentCategoryCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var categoryId = ContentCategoryId.From(request.ContentCategoryId);

			var category = await _repository.GetByIdAsync(categoryId, cancellationToken);

			if (category is null)
				return Result.Failure(ContentCategoryErrors.NotFound);


			var now = _systemClock.UtcNow;

			var updateResult = category.Update(
				request.Name,
				request.JapaneseName,
				request.DisplayOrder,
				request.ThemeColor,
				request.IsActive,
				now,
				request.Description);

			if (updateResult.IsFailure)
				return updateResult;

			return Result.Success();
		}, cancellationToken);
	}
}
