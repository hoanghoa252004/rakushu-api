using Entity = Rakushu.Domain.Entities.Linguistic.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetDependencyRelationshipById;

public sealed record GetDependencyRelationshipByIdQuery(Guid DependencyRelationshipId) : IRequest<Result<DependencyRelationshipDto>>;
