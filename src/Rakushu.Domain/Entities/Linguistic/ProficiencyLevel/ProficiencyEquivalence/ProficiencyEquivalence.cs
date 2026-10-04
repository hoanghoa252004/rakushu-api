using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Linguistic.ProficiencyLevel;

namespace Rakushu.Domain.Entities.Linguistic.ProficiencyLevel.ProficiencyEquivalence;

public sealed partial class ProficiencyEquivalence : Entity<ProficiencyEquivalenceId>
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

	public static Result<ProficiencyEquivalence> Create(
		ProficiencyLevelId sourceLevelId,
		ProficiencyLevelId targetLevelId,
		EquivalenceType type,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt,
		string? note = null,
		string? reference = null)
	{
		return Result.Success(new ProficiencyEquivalence(
			ProficiencyEquivalenceId.Create(),
			sourceLevelId,
			targetLevelId,
			type,
			createdAt,
			updatedAt,
			note,
			reference));
	}

	public Result Update(
		ProficiencyLevelId sourceLevelId,
		ProficiencyLevelId targetLevelId,
		EquivalenceType type,
		DateTimeOffset updatedAt,
		string? note = null,
		string? reference = null)
	{
		SourceLevelId = sourceLevelId;
		TargetLevelId = targetLevelId;
		Type = type;
		Note = note;
		Reference = reference;
		UpdatedAt = updatedAt;
		return Result.Success();
	}
}