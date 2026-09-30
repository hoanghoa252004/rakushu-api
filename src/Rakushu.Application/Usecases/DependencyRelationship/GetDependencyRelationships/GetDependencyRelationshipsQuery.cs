using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;
using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.DependencyRelationship.GetDependencyRelationships;

public sealed record GetDependencyRelationshipsQuery : IRequest<Result<IReadOnlyCollection<DependencyRelationshipDto>>>;
