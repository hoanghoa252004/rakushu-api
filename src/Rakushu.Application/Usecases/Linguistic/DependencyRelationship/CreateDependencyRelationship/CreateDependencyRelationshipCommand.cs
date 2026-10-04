using Entity = Rakushu.Domain.Entities.Linguistic.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.CreateDependencyRelationship;

public sealed record CreateDependencyRelationshipCommand(
	string code,
	string name,
	string vietnameseName,
	string? description
) : IRequest<Result<Guid>>;
