using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;

public sealed partial class ProficiencyFramework : AggregateRoot<ProficiencyFrameworkId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public string? Description { get; private set; }
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ProficiencyLevels:
	private readonly List<Linguistic.ProficiencyLevel.ProficiencyLevel> _proficiencyLevels = new();
	public IReadOnlyCollection<Linguistic.ProficiencyLevel.ProficiencyLevel> ProficiencyLevels => _proficiencyLevels.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private ProficiencyFramework() { }

	private ProficiencyFramework(
		ProficiencyFrameworkId id,
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		Description = description;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<ProficiencyFramework> Create(
		string code,
		string name,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure<ProficiencyFramework>(ProficiencyFrameworkErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure<ProficiencyFramework>(ProficiencyFrameworkErrors.InvalidCode);

		return Result.Success(new ProficiencyFramework(
			ProficiencyFrameworkId.Create(),
			code,
			name,
			isActive,
			createdAt,
			updatedAt,
			description));
	}

	public Result Update(
		string code,
		string name,
		bool isActive,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(ProficiencyFrameworkErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure(ProficiencyFrameworkErrors.InvalidCode);

		Code = code;
		Name = name;
		Description = description;
		IsActive = isActive;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result<Linguistic.ProficiencyLevel.ProficiencyLevel> AddLevel(
		string code,
		string name,
		int sortOrder,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		var level = Linguistic.ProficiencyLevel.ProficiencyLevel.Create(
			code,
			name,
			sortOrder,
			true,
			Id,
			createdAt,
			updatedAt,
			description);

		if (level.IsFailure)
			return level;

		_proficiencyLevels.Add(level.Value);
		UpdatedAt = updatedAt;
		return level;
	}

	public Result UpdateLevel(
		ProficiencyLevelId levelId,
		string code,
		string name,
		int sortOrder,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		var level = _proficiencyLevels.FirstOrDefault(l => l.Id == levelId);
		if (level is null)
			return Result.Failure(ProficiencyLevelErrors.NotFound);

		var result = level.Update(code, name, sortOrder, updatedAt, description);
		if (result.IsFailure)
			return result;

		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result RemoveLevel(ProficiencyLevelId levelId)
	{
		var level = _proficiencyLevels.FirstOrDefault(l => l.Id == levelId);
		if (level is null)
			return Result.Failure(ProficiencyLevelErrors.NotFound);

		_proficiencyLevels.Remove(level);
		UpdatedAt = DateTimeOffset.UtcNow;
		return Result.Success();
	}
}