using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyFramework;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;
using Rakushu.Domain.Entities.User.Profile;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

public sealed partial class ProficiencyLevel : Entity<ProficiencyLevelId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public int SortOrder { get; private set; }
	public string? Description { get; private set; }
	public ProficiencyFrameworkId ProficiencyFrameworkId { get; private set; } = null!;
	public bool IsActive { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
	

	// NAVIGATION PROPERTIES
	// ProficiencyFramework:
	public ProficiencyFramework.ProficiencyFramework Framework { get; private set; } = null!;

	// SourceLevelProficiencyEquivalences:
	private readonly List<ProficiencyEquivalence.ProficiencyEquivalence> _sourceLevelProficiencyEquivalences = new();
	public IReadOnlyCollection<ProficiencyEquivalence.ProficiencyEquivalence> SourceLevelProficiencyEquivalences => _sourceLevelProficiencyEquivalences.AsReadOnly();

	// TargetLevelProficiencyEquivalences:
	private readonly List<ProficiencyEquivalence.ProficiencyEquivalence> _targetLevelProficiencyEquivalences = new();
	public IReadOnlyCollection<ProficiencyEquivalence.ProficiencyEquivalence> TargetLevelProficiencyEquivalences => _targetLevelProficiencyEquivalences.AsReadOnly();

	// TargetLevelProfiles:
	private readonly List<Profile> _targetLevelProfiles = new();
	public IReadOnlyCollection<Profile> TargetLevelProfiles => _targetLevelProfiles.AsReadOnly();

	// CurrentLevelProfiles:
	private readonly List<Profile> _currentLevelProfiles = new();
	public IReadOnlyCollection<Profile> CurrentLevelProfiles => _currentLevelProfiles.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS
	private ProficiencyLevel() { }

	private ProficiencyLevel(
		ProficiencyLevelId id,
		string code,
		string name,
		int sortOrder,
		ProficiencyFrameworkId proficiencyFrameworkId,
		bool isActive,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		Description = description;
		SortOrder = sortOrder;
		ProficiencyFrameworkId = proficiencyFrameworkId;
		IsActive = isActive;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}

	public static Result<ProficiencyLevel> Create(
		string code,
		string name,
		int sortOrder,
		bool isActive,
		ProficiencyFrameworkId proficiencyFrameworkId,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure<ProficiencyLevel>(ProficiencyLevelErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure<ProficiencyLevel>(ProficiencyLevelErrors.InvalidCode);

		return Result.Success(new ProficiencyLevel(
			ProficiencyLevelId.Create(),
			code,
			name,
			sortOrder,
			proficiencyFrameworkId,
			isActive,
			createdAt,
			updatedAt,
			description));
	}

	public Result Update(
		string code,
		string name,
		int sortOrder,
		DateTimeOffset updatedAt,
		string? description = null)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result.Failure(ProficiencyLevelErrors.InvalidName);

		if (string.IsNullOrWhiteSpace(code))
			return Result.Failure(ProficiencyLevelErrors.InvalidCode);

		Code = code;
		Name = name;
		SortOrder = sortOrder;
		Description = description;
		UpdatedAt = updatedAt;
		return Result.Success();
	}

	public Result<ProficiencyEquivalence.ProficiencyEquivalence> AddEquivalence(
		ProficiencyLevelId targetLevelId,
		EquivalenceType type,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? note = null,
		string? reference = null)
	{
		var eq = ProficiencyEquivalence.ProficiencyEquivalence.Create(
			Id,
			targetLevelId,
			type,
			createdAt,
			updatedAt,
			note,
			reference);

		if (eq.IsFailure)
			return eq;

		_sourceLevelProficiencyEquivalences.Add(eq.Value);
		return eq;
	}

	public Result RemoveEquivalence(ProficiencyEquivalence.ProficiencyEquivalenceId equivalenceId)
	{
		var eq = _sourceLevelProficiencyEquivalences.FirstOrDefault(e => e.Id == equivalenceId);
		if (eq is null)
			return Result.Failure(ProficiencyEquivalence.ProficiencyEquivalenceErrors.NotFound);

		_sourceLevelProficiencyEquivalences.Remove(eq);
		return Result.Success();
	}
}