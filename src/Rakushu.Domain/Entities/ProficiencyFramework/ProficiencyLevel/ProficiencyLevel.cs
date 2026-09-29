using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.User.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

public sealed class ProficiencyLevel : Entity<ProficiencyLevelId>
{
	public string Code { get; private set; } = null!;
	public string Name { get; private set; } = null!;
	public int SortOrder { get; private set; }
	public string? Description { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// ProficiencyFramework:
	public ProficiencyFramework Framework { get; private set; } = null!;

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
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? description = null)
		: base(id)
	{
		Code = code;
		Name = name;
		Description = description;
		SortOrder = sortOrder;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
