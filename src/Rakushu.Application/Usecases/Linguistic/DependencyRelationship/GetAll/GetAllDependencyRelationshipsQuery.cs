using MediatR;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetAll;

public sealed record GetAllDependencyRelationshipsQuery : IRequest<List<DependencyRelationshipDto>>;

public sealed class GetAllDependencyRelationshipsHandler : IRequestHandler<GetAllDependencyRelationshipsQuery, List<DependencyRelationshipDto>>
{
	private readonly IDependencyRelationshipRepository _repository;

	public GetAllDependencyRelationshipsHandler(IDependencyRelationshipRepository repository)
	{
		_repository = repository;
	}

	public async Task<List<DependencyRelationshipDto>> Handle(GetAllDependencyRelationshipsQuery query, CancellationToken cancellationToken)
	{
		var entities = await _repository.GetAllAsync(cancellationToken);
		return entities.Select(e => new DependencyRelationshipDto
		{
			Id = e.Id.Value,
			Code = e.Code,
			Name = e.Name,
			JapaneseName = e.JapaneseName,
			Description = e.Description
		}).ToList();
	}
}
