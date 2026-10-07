using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Delete;

public sealed record DeleteDependencyRelationshipCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteDependencyRelationshipHandler : IRequestHandler<DeleteDependencyRelationshipCommand, Result>
{
	private readonly IDependencyRelationshipRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteDependencyRelationshipHandler(IDependencyRelationshipRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteDependencyRelationshipCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = DependencyRelationshipId.From(command.Id);
			var entity = await _repository.GetByIdAsync(id, cancellationToken);

			if (entity is null)
				return Result.Failure(LinguisticErrors.NotFound);

			_repository.Delete(entity);
			return Result.Success();
		});
	}
}
