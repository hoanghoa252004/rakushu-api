using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.ContentCategory;
using Rakushu.Domain.Entities.Feature;

namespace Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;

internal sealed class CreateContentCategoryHandler : IRequestHandler<CreateContentCategoryCommand, Result<Guid>>
{
	private readonly IContentCategoryRepository _repository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _systemClock;

	public CreateContentCategoryHandler(IContentCategoryRepository repository, IUnitOfWork unitOfWork, ISystemClock systemClock)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
		_systemClock = systemClock;
	}

	public async Task<Result<Guid>> Handle(CreateContentCategoryCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			// Check if Category with same code already exists
			var existingCategory = await _repository.GetByCodeAsync(request.Code, cancellationToken);

			if (existingCategory is not null)
			{
				return Result.Failure<Guid>(ContentCategoryErrors.DuplicateCode);
			}

			var now = _systemClock.UtcNow;

			var categoryResult = Domain.Entities.ContentCategory.ContentCategory.Create(
				request.Slug,
				request.Code,
				request.Name,
				request.JapaneseName,
				request.DisplayOrder,
				request.ThemeColor,
				request.IsActive,
				now,
				request.Description);

			if (categoryResult.IsFailure)
				return Result.Failure<Guid>(categoryResult.Error);

			_repository.Add(categoryResult.Value);

			return Result.Success(categoryResult.Value.Id.Value);
		}, cancellationToken);
	}
}
