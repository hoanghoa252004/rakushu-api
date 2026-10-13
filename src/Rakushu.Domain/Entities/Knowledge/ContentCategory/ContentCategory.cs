using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User.Profile.Interest;

namespace Rakushu.Domain.Entities.Knowledge.ContentCategory;

public sealed partial class ContentCategory : AggregateRoot<ContentCategoryId>
{
	public string Slug { get; private set; } = null!;
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string JapaneseName { get; private set; } = null!;
	public string? Description { get; private set; }
	public int DisplayOrder { get; private set; }
	public string ThemeColor { get; private set; } = null!;
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ContentKnowledgeCollections
	private readonly List<Knowledge.ContentKnowledgeCollection.ContentKnowledgeCollection> _contentKnowledgeCollections = new();
	public IReadOnlyCollection<Knowledge.ContentKnowledgeCollection.ContentKnowledgeCollection> ContentKnowledgeCollections => _contentKnowledgeCollections.AsReadOnly();

	// Interests
	private readonly List<Interest> _interests = new();
	public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();

	// Videos:
	private readonly List<Material.Video.Video> _videos = new();
	public IReadOnlyCollection<Material.Video.Video> Videos => _videos.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private ContentCategory() { }

	private ContentCategory(
		ContentCategoryId id,
		string slug,
		string code,
		string name,
		string japaneseName,
		int displayOrder,
		string themeColor,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		bool isActive,
		string? description = null)
		: base(id)
	{
		Slug = slug;
		Code = code;
		Name = name;
		JapaneseName = japaneseName;
		DisplayOrder = displayOrder;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		IsActive = isActive;
		Description = description;
		ThemeColor = themeColor;
	}

	public static Result<ContentCategory> Create(
		string slug,
		string code,
		string name,
		string japaneseName,
		int displayOrder,
		string themeColor,
		bool isActive,
		DateTimeOffset createdAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(japaneseName))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidCode);

		if(displayOrder <= 0)
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidDisplayOrder);

		if (string.IsNullOrWhiteSpace(slug))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidThemeColor);

		if (string.IsNullOrWhiteSpace(themeColor))
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidThemeColor);

		return Result.Success(new ContentCategory(
			ContentCategoryId.Create(),
			slug,
			code,
			name,
			japaneseName,
			displayOrder,
			themeColor,
			createdAt,
			createdAt,
			isActive,
			description));
	}

	public Result Update(
		string name,
		string japaneseName,
		int displayOrder,
		string themeColor,
		bool isActive,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(themeColor))
			return Result.Failure(ContentCategoryErrors.InvalidThemeColor);

		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(ContentCategoryErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(japaneseName))
			return Result.Failure(ContentCategoryErrors.InvalidName);

		if (displayOrder <= 0)
			return Result.Failure<ContentCategory>(ContentCategoryErrors.InvalidDisplayOrder);

		Name = name;
		JapaneseName = japaneseName;
		DisplayOrder = displayOrder;
		Description = description;
		ThemeColor = themeColor;
		IsActive = isActive;
		UpdatedAt = updatedAt;

		return Result.Success();
	}
}