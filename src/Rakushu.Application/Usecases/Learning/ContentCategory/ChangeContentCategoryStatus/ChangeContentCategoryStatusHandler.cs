using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.ChangeContentCategoryStatus;

internal sealed class ChangeContentCategoryStatusHandler : IRequestHandler<ChangeContentCategoryStatusCommand, Result>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public ChangeContentCategoryStatusHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(ChangeContentCategoryStatusCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var category = await _repository.GetByIdAsync(ContentCategoryId.From(request.ContentCategoryId), cancellationToken);
			if (category is null)
				return Result.Failure(ContentCategoryErrors.NotFound);

			var now = DateTimeOffset.UtcNow;
			var changeResult = category.ChangeStatus(request.NewStatus, now);

			if (changeResult.IsFailure)
				return changeResult;

			return Result.Success();
		}, cancellationToken);
	}
}
