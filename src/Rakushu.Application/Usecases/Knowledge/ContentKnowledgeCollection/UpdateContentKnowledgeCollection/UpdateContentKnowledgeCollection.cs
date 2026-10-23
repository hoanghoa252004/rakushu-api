using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.UpdateContentKnowledgeCollection;

public sealed record UpdateContentKnowledgeCollectionCommand(
	Guid ContentKnowledgeCollectionId,
	int Priority,
	bool IsActive
) : IRequest<Result>;

internal sealed class UpdateContentKnowledgeCollectionValidator : AbstractValidator<UpdateContentKnowledgeCollectionCommand>
{
	public UpdateContentKnowledgeCollectionValidator()
	{
		RuleFor(x => x.ContentKnowledgeCollectionId).NotEmpty().WithMessage("ID is required");
		RuleFor(x => x.Priority).GreaterThan(0).WithMessage("Priority must be greater than 0");
	}
}

internal sealed class UpdateContentKnowledgeCollectionHandler : IRequestHandler<UpdateContentKnowledgeCollectionCommand, Result>
{
	private readonly IContentKnowledgeCollectionRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateContentKnowledgeCollectionHandler(IContentKnowledgeCollectionRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateContentKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var item = await _repository.GetByIdAsync(ContentKnowledgeCollectionId.From(request.ContentKnowledgeCollectionId), cancellationToken);

			if (item is null)
				return Result.Failure(ContentKnowledgeCollectionErrors.NotFound);

			var siblings = await _repository.GetByContentCategoryIdAsync(item.ContentCategoryId, cancellationToken);

			// Priority must stay within the existing sequence (no gap)
			if (request.Priority > siblings.Count)
				return Result.Failure(ContentKnowledgeCollectionErrors.InvalidPriority);

			var oldPriority = item.Priority;
			var newPriority = request.Priority;

			// Move the item and shift the others so the sequence stays unique and continuous
			if (newPriority < oldPriority)
			{
				foreach (var other in siblings.Where(x => x.Id != item.Id && x.Priority >= newPriority && x.Priority < oldPriority))
					other.SetPriority(other.Priority + 1);
			}
			else if (newPriority > oldPriority)
			{
				foreach (var other in siblings.Where(x => x.Id != item.Id && x.Priority > oldPriority && x.Priority <= newPriority))
					other.SetPriority(other.Priority - 1);
			}

			item.SetPriority(newPriority);
			item.SetActive(request.IsActive);

			return Result.Success();
		}, cancellationToken);
	}
}
