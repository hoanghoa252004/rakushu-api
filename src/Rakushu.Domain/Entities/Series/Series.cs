using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.ContentCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Series;

public sealed class Series : AggregateRoot<SeriesId>
{
	public string Slug { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public int DisplayOrder { get; private set; }
	public SeriesSource SourceType { get; private set; }
	public ContentCategoryId ContentCategoryId { get; private set; } = null!;
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentCategory:
	public ContentCategory.ContentCategory ContentCategory { get; private set; } = null!;

	// Videos:
	private readonly List<Video.Video> _videos = new();
	public IReadOnlyCollection<Video.Video> Videos => _videos.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Series() { }

	private Series(
		SeriesId id,
		string slug,
		string name,
		int displayOrder,
		SeriesSource sourceType,
		ContentCategoryId contentCategoryId,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Slug = slug;
		Name = name;
		DisplayOrder = displayOrder;
		SourceType = sourceType;
		ContentCategoryId = contentCategoryId;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

}