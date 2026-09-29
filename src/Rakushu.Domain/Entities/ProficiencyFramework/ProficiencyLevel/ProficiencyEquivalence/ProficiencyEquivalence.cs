using Rakushu.Domain.Common;
using Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

public sealed class ProficiencyEquivalence : Entity<ProficiencyEquivalenceId>
{
	public ProficiencyLevelId SourceLevelId { get; private set; } = null!;
	public ProficiencyLevelId TargetLevelId { get; private set; } = null!;
	public EquivalenceType Type { get; private set; }
	public string? Note { get; private set; }
	public string? Reference { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	// SourceProficiencyLevel
	public ProficiencyLevel SourceLevel { get; private set; } = null!;

	// TargetProficiencyLevel
	public ProficiencyLevel TargetLevel { get; private set; } = null!;

	private ProficiencyEquivalence() { }

	private ProficiencyEquivalence(
		ProficiencyEquivalenceId id,
		ProficiencyLevelId sourceLevelId,
		ProficiencyLevelId targetLevelId,
		EquivalenceType type,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? note = null,
		string? reference = null)
		: base(id)
	{
		SourceLevelId = sourceLevelId;
		TargetLevelId = targetLevelId;
		Type = type;
		Note = note;
		Reference = reference;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
	}
}
