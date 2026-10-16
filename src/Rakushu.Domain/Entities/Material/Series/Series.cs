using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.Identity.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Series;

public sealed class Series : AggregateRoot<SeriesId>
{
	public string Slug { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public int DisplayOrder { get; private set; }
	public SeriesSource SourceType { get; private set; }
	public UserId UserId { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// CreatedBy
	public User User { get; private set; } = null!;

	// Videos:
	private readonly List<Video.Video> _videos = new();
	public IReadOnlyCollection<Video.Video> Videos => _videos.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private Series() { }

	private Series(
		SeriesId id,
		UserId userId,
		string slug,
		string name,
		int displayOrder,
		SeriesSource sourceType,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		UserId = userId;
		Slug = slug;
		Name = name;
		DisplayOrder = displayOrder;
		SourceType = sourceType;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Series Create(
		UserId userId,
		string slug,
		string name,
		int displayOrder,
		SeriesSource sourceType,
		bool isActive,
		string? description = null)
	{
		var now = DateTimeOffset.UtcNow;
		return new Series(
			SeriesId.Create(),
			userId,
			slug,
			name,
			displayOrder,
			sourceType,
			isActive,
			now,
			now,
			description);
	}

	public void Update(
		string name,
		string? description,
		int displayOrder,
		bool isActive)
	{
		Name = name;
		Description = description;
		DisplayOrder = displayOrder;
		IsActive = isActive;
		UpdatedAt = DateTimeOffset.UtcNow;
	}
}