using Entity = Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship.DependencyRelationship;

using Rakushu.Domain.Entities.LinguisticMetadata.DependencyRelationship;

namespace Rakushu.Application.Usecases.DependencyRelationship;

public sealed record DependencyRelationshipDto(
	string Code,
	string Name,
	string VietnameseName,
	string? Description)
{
	public static DependencyRelationshipDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.VietnameseName,
			e.Description);
}
