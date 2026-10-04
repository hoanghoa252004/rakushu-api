using Entity = Rakushu.Domain.Entities.Linguistic.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.UpdateDependencyRelationship;

public sealed record UpdateDependencyRelationshipCommand(
	Guid DependencyRelationshipId,
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result>;
