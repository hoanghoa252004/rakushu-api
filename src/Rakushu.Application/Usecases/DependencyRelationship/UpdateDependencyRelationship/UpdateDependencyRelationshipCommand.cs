using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship.UpdateDependencyRelationship;

public sealed record UpdateDependencyRelationshipCommand(
	Guid DependencyRelationshipId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
