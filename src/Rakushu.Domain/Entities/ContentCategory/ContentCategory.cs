using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Domain.Entities.ContentCategory;

public sealed partial class ContentCategory : AggregateRoot<ContentCategoryId>
{
	public string Slug { get; private set; } = null!;
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public ContentCategoryId? ParentId { get; private set; }
	public int Level { get; private set; }
	public int DisplayOrder { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentCategory (Self-referencing)
	public ContentCategory? Parent { get; private set; }
	private readonly List<ContentCategory> _children = new();
	public IReadOnlyCollection<ContentCategory> Children => _children.AsReadOnly();

	// ContentProcessingPolicies
	private readonly List<ContentProcessingPolicy.ContentProcessingPolicy> _contentProcessingPolicies = new();
	public IReadOnlyCollection<ContentProcessingPolicy.ContentProcessingPolicy> ContentProcessingPolicies => _contentProcessingPolicies.AsReadOnly();

	// Interests
	private readonly List<Interest> _interests = new();
	public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();

	// Videos:
	private readonly List<Video.Video> _videos = new();
	public IReadOnlyCollection<Video.Video> Videos => _videos.AsReadOnly();

	// Series:
	private readonly List<Series.Series> _series = new();
	public IReadOnlyCollection<Series.Series> Series => _series.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private ContentCategory() { }

	private ContentCategory(
		ContentCategoryId id,
		string slug,
		string code,
		string name,
		int level,
		int displayOrder,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		ContentCategoryId? parentId = null,
		string? description = null)
		: base(id)
	{
		Slug = slug;
		Code = code;
		Name = name;
		Level = level;
		DisplayOrder = displayOrder;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Description = description;
		ParentId = parentId;
	}

	public static Result<ContentCategory> Create(
		string slug,
		string code,
		string name,
		int level,
		int displayOrder,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		ContentCategoryId? parentId = null,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidCode);

		return Result.Success(new ContentCategory(
			ContentCategoryId.Create(),
			slug,
			code,
			name,
			level,
			displayOrder,
			isActive,
			createdAt,
			updatedAt,
			parentId,
			description));
	}

	public Result Update(
		string slug,
		string code,
		string name,
		int level,
		int displayOrder,
		bool isActive,
		DateTimeOffset updatedAt,
		ContentCategoryId? parentId = null,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(ContentCategoryErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure(ContentCategoryErrors.InvalidCode);

		Slug = slug;
		Code = code;
		Name = name;
		Level = level;
		DisplayOrder = displayOrder;
		IsActive = isActive;
		ParentId = parentId;
		Description = description;
		UpdatedAt = updatedAt;

		return Result.Success();
	}
}