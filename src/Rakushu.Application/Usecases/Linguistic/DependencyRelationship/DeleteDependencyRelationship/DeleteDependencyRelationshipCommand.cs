using Entity = Rakushu.Domain.Entities.Linguistic.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.DeleteDependencyRelationship;

public sealed record DeleteDependencyRelationshipCommand(Guid DependencyRelationshipId) : IRequest<Result>;
