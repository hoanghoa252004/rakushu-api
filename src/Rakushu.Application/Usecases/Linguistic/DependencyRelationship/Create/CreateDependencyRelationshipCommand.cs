using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using DependencyRelationshipEntity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Create;

public sealed record CreateDependencyRelationshipCommand(
	string Code,
	string Name,
	string JapaneseName,
	string? Description) : IRequest<Result<Guid>>;

internal sealed class CreateDependencyRelationshipHandler : IRequestHandler<CreateDependencyRelationshipCommand, Result<Guid>>
{
	private readonly IDependencyRelationshipRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateDependencyRelationshipHandler(IDependencyRelationshipRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateDependencyRelationshipCommand command, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var id = DependencyRelationshipId.Create();
			var entity = DependencyRelationshipEntity.Create(
				id,
				command.Code,
				command.Name,
				command.JapaneseName,
				command.Description);

			_repository.Add(entity);
			return Result.Success(id.Value);
		});
	}
}
