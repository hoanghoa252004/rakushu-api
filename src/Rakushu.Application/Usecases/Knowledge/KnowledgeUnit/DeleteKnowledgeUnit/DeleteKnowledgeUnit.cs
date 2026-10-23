using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

namespace Rakushu.Application.Usecases.Knowledge.KnowledgeUnit.DeleteKnowledgeUnit;

public sealed record DeleteKnowledgeUnitCommand(Guid KnowledgeUnitId) : IRequest<Result>;

internal sealed class DeleteKnowledgeUnitHandler : IRequestHandler<DeleteKnowledgeUnitCommand, Result>
{
	private readonly IKnowledgeUnitRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteKnowledgeUnitHandler(IKnowledgeUnitRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteKnowledgeUnitCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var unit = await _repository.GetByIdAsync(KnowledgeUnitId.From(request.KnowledgeUnitId), cancellationToken);

			if (unit is null)
				return Result.Failure(KnowledgeUnitErrors.NotFound);

			_repository.Delete(unit);

			return Result.Success();
		}, cancellationToken);
	}
}
