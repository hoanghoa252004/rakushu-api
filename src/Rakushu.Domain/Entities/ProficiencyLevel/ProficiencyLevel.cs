using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Domain.Entities.ProficiencyLevel;

public sealed partial class ProficiencyLevel : Entity<ProficiencyLevelId>
{
	public string Code { get; private set; } = null!; // N5
	public string Name { get; private set; } = null!; // Sơ cấp
	public string JapaneseName { get; private set; } = null!; // 初級
	public int SortOrder { get; private set; } // 1
	public string? Description { get; private set; } // XXX
	public bool IsActive { get; private set; } // TRUE
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
	

	// NAVIGATION PROPERTIES
	// ProfileLevels:
	private readonly List<Profile> _profileLevels = new();
	public IReadOnlyCollection<Profile> ProfileLevels => _profileLevels.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private ProficiencyLevel() { }

	private ProficiencyLevel(
		ProficiencyLevelId id,
		string code,
		string name,
		string japaneseName,
		int sortOrder,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		JapaneseName = japaneseName;
		Description = description;
		SortOrder = sortOrder;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<ProficiencyLevel> Create(
		string code,
		string name,
		string japaneseName,
		int sortOrder,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure<ProficiencyLevel>(ProficiencyLevelErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure<ProficiencyLevel>(ProficiencyLevelErrors.InvalidCode);

		if (string.IsNullOrWhiteSpace(japaneseName))
			return Result.Failure<ProficiencyLevel>(ProficiencyLevelErrors.InvalidName);

		return Result.Success(new ProficiencyLevel(
			ProficiencyLevelId.Create(),
			code,
			name,
			japaneseName,
			sortOrder,
			isActive,
			createdAt,
			updatedAt,
			description));
	}

	public Result Update(
		string code,
		string name,
		string japaneseName,
		int sortOrder,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(ProficiencyLevelErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure(ProficiencyLevelErrors.InvalidCode);

		if (string.IsNullOrWhiteSpace(japaneseName))
			return Result.Failure(ProficiencyLevelErrors.InvalidName);

		Code = code;
		Name = name;
		JapaneseName = japaneseName;
		SortOrder = sortOrder;
		Description = description;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

}