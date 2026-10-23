using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.DeleteContentKnowledgeCollection;

public sealed record DeleteContentKnowledgeCollectionCommand(Guid ContentKnowledgeCollectionId) : IRequest<Result>;

internal sealed class DeleteContentKnowledgeCollectionHandler : IRequestHandler<DeleteContentKnowledgeCollectionCommand, Result>
{
	private readonly IContentKnowledgeCollectionRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteContentKnowledgeCollectionHandler(IContentKnowledgeCollectionRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteContentKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var item = await _repository.GetByIdAsync(ContentKnowledgeCollectionId.From(request.ContentKnowledgeCollectionId), cancellationToken);

			if (item is null)
				return Result.Failure(ContentKnowledgeCollectionErrors.NotFound);

			var siblings = await _repository.GetByContentCategoryIdAsync(item.ContentCategoryId, cancellationToken);
			var removedPriority = item.Priority;

			_repository.Delete(item);

			// Recalculate priorities of the remaining items
			foreach (var other in siblings.Where(x => x.Id != item.Id && x.Priority > removedPriority))
				other.SetPriority(other.Priority - 1);

			return Result.Success();
		}, cancellationToken);
	}
}
