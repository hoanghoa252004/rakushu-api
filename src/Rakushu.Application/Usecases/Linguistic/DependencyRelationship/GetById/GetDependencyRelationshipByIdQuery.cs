using MediatR;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.Common;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetById;

public sealed record GetDependencyRelationshipByIdQuery(Guid Id) : IRequest<DependencyRelationshipDto?>;

public sealed class GetDependencyRelationshipByIdHandler : IRequestHandler<GetDependencyRelationshipByIdQuery, DependencyRelationshipDto?>
{
	private readonly IDependencyRelationshipRepository _repository;

	public GetDependencyRelationshipByIdHandler(IDependencyRelationshipRepository repository)
	{
		_repository = repository;
	}

	public async Task<DependencyRelationshipDto?> Handle(GetDependencyRelationshipByIdQuery query, CancellationToken cancellationToken)
	{
		var id = DependencyRelationshipId.From(query.Id);
		var entity = await _repository.GetByIdAsync(id, cancellationToken);
		if (entity == null) return null;

		return new DependencyRelationshipDto
		{
			Id = entity.Id.Value,
			Code = entity.Code,
			Name = entity.Name,
			JapaneseName = entity.JapaneseName,
			Description = entity.Description
		};
	}
}
