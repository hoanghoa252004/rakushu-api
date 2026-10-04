using Entity = Rakushu.Domain.Entities.Linguistic.DependencyRelationship.DependencyRelationship;

namespace Rakushu.Application.Usecases.Linguistic.DependencyRelationship;

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
