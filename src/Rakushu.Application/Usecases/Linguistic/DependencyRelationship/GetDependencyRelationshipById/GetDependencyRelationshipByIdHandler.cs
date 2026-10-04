using MediatR;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetDependencyRelationshipById;

internal sealed class GetDependencyRelationshipByIdHandler : IRequestHandler<GetDependencyRelationshipByIdQuery, Result<DependencyRelationshipDto>>
{
	private readonly IDependencyRelationshipRepository _repository;

	public GetDependencyRelationshipByIdHandler(IDependencyRelationshipRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<DependencyRelationshipDto>> Handle(GetDependencyRelationshipByIdQuery request, CancellationToken cancellationToken)
	{
		var rel = await _repository.GetByIdAsync(DependencyRelationshipId.From(request.DependencyRelationshipId), cancellationToken);
		if (rel is null)
			return Result.Failure<DependencyRelationshipDto>(Error.NotFound("DEPENDENCY_RELATIONSHIP.NOT_FOUND", "Dependency relationship not found."));

		return Result.Success(DependencyRelationshipDto.FromEntity(rel));
	}
}