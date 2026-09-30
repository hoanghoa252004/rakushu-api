using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.DependencyRelationship.GetDependencyRelationshipById;

public sealed record GetDependencyRelationshipByIdQuery(Guid DependencyRelationshipId) : IRequest<Result<DependencyRelationshipDto>>;
