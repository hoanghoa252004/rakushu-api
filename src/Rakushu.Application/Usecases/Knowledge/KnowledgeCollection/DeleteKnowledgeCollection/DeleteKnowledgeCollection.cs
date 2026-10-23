using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeCollection.DeleteKnowledgeCollection;

public sealed record DeleteKnowledgeCollectionCommand(Guid KnowledgeCollectionId) : IRequest<Result>;

internal sealed class DeleteKnowledgeCollectionHandler : IRequestHandler<DeleteKnowledgeCollectionCommand, Result>
{
	private readonly IKnowledgeCollectionRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteKnowledgeCollectionHandler(IKnowledgeCollectionRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteKnowledgeCollectionCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var collection = await _repository.GetByIdAsync(KnowledgeCollectionId.From(request.KnowledgeCollectionId), cancellationToken);

			if (collection is null)
				return Result.Failure(KnowledgeCollectionErrors.NotFound);

			if (collection.ContentKnowledgeCollections.Any())
				return Result.Failure(KnowledgeCollectionErrors.IsInUse);

			_repository.Delete(collection);

			return Result.Success();
		}, cancellationToken);
	}
}
