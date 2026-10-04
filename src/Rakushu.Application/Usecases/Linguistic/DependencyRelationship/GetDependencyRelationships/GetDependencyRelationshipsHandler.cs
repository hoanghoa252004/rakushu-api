using MediatR;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetDependencyRelationships;

internal sealed class GetDependencyRelationshipsHandler : IRequestHandler<GetDependencyRelationshipsQuery, Result<IReadOnlyCollection<DependencyRelationshipDto>>>
{
	private readonly IDependencyRelationshipRepository _repository;

	public GetDependencyRelationshipsHandler(IDependencyRelationshipRepository repository)
	{
		_repository = repository;
	}

	public async Task<Result<IReadOnlyCollection<DependencyRelationshipDto>>> Handle(GetDependencyRelationshipsQuery request, CancellationToken cancellationToken)
	{
		var list = await _repository.GetAllAsync(cancellationToken);
		return Result.Success<IReadOnlyCollection<DependencyRelationshipDto>>(list.Select(DependencyRelationshipDto.FromEntity).ToArray());
	}
}