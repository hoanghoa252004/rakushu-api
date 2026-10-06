using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using DependencyRelationshipEntity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Update;

public sealed record UpdateDependencyRelationshipCommand(
	Guid Id,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result>;

internal sealed class UpdateDependencyRelationshipHandler : IRequestHandler<UpdateDependencyRelationshipCommand, Result>
{
	private readonly IDependencyRelationshipRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateDependencyRelationshipHandler(IDependencyRelationshipRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(UpdateDependencyRelationshipCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = DependencyRelationshipId.From(command.Id);
			var entity = await _repository.GetByIdAsync(id, cancellationToken);

			if (entity is null)
				return Result.Failure(LinguisticErrors.NotFound);

			entity.Update(command.Name, command.JapaneseName, command.Description);
			_repository.Update(entity);
			return Result.Success();
		});
	}
}
