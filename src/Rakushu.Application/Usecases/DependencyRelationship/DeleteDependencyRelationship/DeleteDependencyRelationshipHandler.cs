using MediatR;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship.DeleteDependencyRelationship;

internal sealed class DeleteDependencyRelationshipHandler : IRequestHandler<DeleteDependencyRelationshipCommand, Result>
{
	private readonly IDependencyRelationshipRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public DeleteDependencyRelationshipHandler(IDependencyRelationshipRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(DeleteDependencyRelationshipCommand request, CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var rel = await _repository.GetByIdAsync(DependencyRelationshipId.From(request.DependencyRelationshipId), cancellationToken);
			if (rel is null)
				return Result.Failure(Error.NotFound("DEPENDENCY_RELATIONSHIP.NOT_FOUND", "Dependency relationship not found."));

			_repository.Delete(rel);
			return Result.Success();
		}, cancellationToken);
	}
}