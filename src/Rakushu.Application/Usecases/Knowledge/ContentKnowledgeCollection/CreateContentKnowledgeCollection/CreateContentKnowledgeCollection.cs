using FluentValidation;
using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.CreateContentKnowledgeCollection;

public sealed record CreateContentKnowledgeCollectionCommand(
	Guid ContentCategoryId,
	Guid KnowledgeCollectionId,
	int Priority
) : IRequest<Result<Guid>>;

internal sealed class CreateContentKnowledgeCollectionValidator : AbstractValidator<CreateContentKnowledgeCollectionCommand>
{
	public CreateContentKnowledgeCollectionValidator()
	{
		RuleFor(x => x.ContentCategoryId).NotEmpty().WithMessage("Content category ID is required");
		RuleFor(x => x.KnowledgeCollectionId).NotEmpty().WithMessage("Knowledge collection ID is required");
		RuleFor(x => x.Priority).GreaterThan(0).WithMessage("Priority must be greater than 0");
	}
}

internal sealed class CreateContentKnowledgeCollectionHandler : IRequestHandler<CreateContentKnowledgeCollectionCommand, Result<Guid>>
{
	private readonly IContentKnowledgeCollectionRepository _repository;
	private readonly IContentCategoryRepository _categoryRepository;
	private readonly IKnowledgeCollectionRepository _collectionRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateContentKnowledgeCollectionHandler(
		IContentKnowledgeCollectionRepository repository,
		IContentCategoryRepository categoryRepository,
		IKnowledgeCollectionRepository collectionRepository,
		IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_categoryRepository = categoryRepository;
		_collectionRepository = collectionRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateContentKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var categoryId = ContentCategoryId.From(request.ContentCategoryId);
			var collectionId = KnowledgeCollectionId.From(request.KnowledgeCollectionId);

			if (await _categoryRepository.GetByIdAsync(categoryId, cancellationToken) is null)
				return Result.Failure<Guid>(ContentKnowledgeCollectionErrors.ContentCategoryNotFound);

			if (await _collectionRepository.GetByIdAsync(collectionId, cancellationToken) is null)
				return Result.Failure<Guid>(ContentKnowledgeCollectionErrors.KnowledgeCollectionNotFound);

			var siblings = await _repository.GetByContentCategoryIdAsync(categoryId, cancellationToken);

			if (siblings.Any(x => x.KnowledgeCollectionId.Value == request.KnowledgeCollectionId))
				return Result.Failure<Guid>(ContentKnowledgeCollectionErrors.AlreadyAttached);

			// Priority must continue the existing sequence (no duplicate, no gap)
			if (request.Priority != siblings.Count + 1)
				return Result.Failure<Guid>(ContentKnowledgeCollectionErrors.InvalidPriority);

			var item = Domain.Entities.Knowledge.ContentKnowledgeCollection.ContentKnowledgeCollection.Create(
				categoryId, collectionId, request.Priority);

			_repository.Add(item);

			return Result.Success(item.Id.Value);
		}, cancellationToken);
	}
}
