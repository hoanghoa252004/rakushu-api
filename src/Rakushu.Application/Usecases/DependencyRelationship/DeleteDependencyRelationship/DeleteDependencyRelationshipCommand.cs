using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship.DeleteDependencyRelationship;

public sealed record DeleteDependencyRelationshipCommand(Guid DependencyRelationshipId) : IRequest<Result>;
