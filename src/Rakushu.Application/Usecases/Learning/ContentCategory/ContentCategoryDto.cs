using Entity = Rakushu.Domain.Entities.ContentCategory.ContentCategory;

using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Application.Usecases.Learning.ContentCategory;

public sealed record ContentCategoryDto(
	Guid Id,
	string Slug,
	string Code,
	string Name,
	string JapaneseName,
	string? Description,
	Guid? ParentId,
	int Level,
	int DisplayOrder,
	ContentCategoryStatus Status)
{
	public static ContentCategoryDto FromEntity(Entity e) =>
		new(
			e.Id.Value,
			e.Slug,
			e.Code,
			e.Name,
			e.JapaneseName,
			e.Description,
			e.ParentId is null ? null : e.ParentId.Value,
			e.Level,
			e.DisplayOrder,
			e.Status);
}
