using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship.CreateDependencyRelationship;

internal sealed class CreateDependencyRelationshipHandler : IRequestHandler<CreateDependencyRelationshipCommand, Result<Guid>>
{
	private readonly IDependencyRelationshipRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateDependencyRelationshipHandler(IDependencyRelationshipRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(CreateDependencyRelationshipCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var rel = Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship.Create(
				DependencyRelationshipId.Create(),
				request.code,
				request.name,
				request.vietnameseName,
				request.description);

			_repository.Add(rel);
			return Result.Success(rel.Id.Value);
		}, cancellationToken);
	}
}