using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship.CreateDependencyRelationship;

public sealed record CreateDependencyRelationshipCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
