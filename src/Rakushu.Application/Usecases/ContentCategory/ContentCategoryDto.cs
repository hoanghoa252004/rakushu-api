using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;

using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.ContentCategory;

public sealed record ContentCategoryDto(
	string Slug,
	string Code,
	string Name,
	string? Description,
	Guid? ParentId,
	int Level,
	int DisplayOrder,
	bool IsActive)
{
	public static ContentCategoryDto FromEntity(Entity e) =>
		new(
			e.Slug,
			e.Code,
			e.Name,
			e.Description,
			e.ParentId is null ? null : e.ParentId.Value,
			e.Level,
			e.DisplayOrder,
			e.IsActive);
}
