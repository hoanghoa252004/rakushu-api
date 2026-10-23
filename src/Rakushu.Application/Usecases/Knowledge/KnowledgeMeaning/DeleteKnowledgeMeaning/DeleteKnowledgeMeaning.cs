using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeMeaning.DeleteKnowledgeMeaning;

public sealed record DeleteKnowledgeMeaningCommand(Guid KnowledgeMeaningId) : IRequest<Result>;

internal sealed class DeleteKnowledgeMeaningHandler : IRequestHandler<DeleteKnowledgeMeaningCommand, Result>
{
	private readonly IKnowledgeMeaningRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteKnowledgeMeaningHandler(IKnowledgeMeaningRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteKnowledgeMeaningCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var meaning = await _repository.GetByIdAsync(KnowledgeMeaningId.From(request.KnowledgeMeaningId), cancellationToken);

			if (meaning is null)
				return Result.Failure(KnowledgeMeaningErrors.NotFound);

			if (meaning.LearningUnits.Any())
				return Result.Failure(KnowledgeMeaningErrors.IsInUse);

			_repository.Delete(meaning);

			return Result.Success();
		}, cancellationToken);
	}
}
