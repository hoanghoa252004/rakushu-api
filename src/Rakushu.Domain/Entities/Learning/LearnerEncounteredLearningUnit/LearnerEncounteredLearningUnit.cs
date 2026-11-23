using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounteredLearningUnit;

public sealed class LearnerEncounteredLearningUnit : AggregateRoot<LearnerEncounteredLearningUnitId>
{
	/// <summary>
	/// Minimum gap between replays of the exact same sentence/LearningUnit before it counts as a new
	/// independent effective encounter (Anti-Massed Repetition Contextual Capping rule).
	/// </summary>
	public static readonly TimeSpan EffectiveEncounterCooldown = TimeSpan.FromMinutes(30);

	public UserId UserId { get; private set; } = null!;
	public LearningUnitId LearningUnitId { get; private set; } = null!;
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public KnowledgeUnitId KnowledgeUnitId { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;

	/// <summary>
	/// Number of effective encounters across distinct viewing sessions (capped to +1 per session for the same sentence).
	/// </summary>
	public int EncounterCount { get; private set; }

	/// <summary>
	/// Total raw audio playback count of this sentence, including immediate loops/replays in the same session.
	/// </summary>
	public int RawPlaybackCount { get; private set; }

	/// <summary>
	/// JSONB array of encounter provenance sources: [{ "videoId", "videoTitle", "timestampSeconds", "sentence" }].
	/// </summary>
	public string EncounterSourcesJson { get; private set; } = "[]";

	public DateTimeOffset FirstEncounteredAt { get; private set; }
	public DateTimeOffset LastEncounteredAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public LearningUnit LearningUnit { get; private set; } = null!;
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public KnowledgeUnit KnowledgeUnit { get; private set; } = null!;
	public Video Video { get; private set; } = null!;

	private LearnerEncounteredLearningUnit()
	{
	}

	private LearnerEncounteredLearningUnit(LearnerEncounteredLearningUnitId id) : base(id)
	{
	}

	public static Result<LearnerEncounteredLearningUnit> Create(
		UserId userId,
		LearningUnitId learningUnitId,
		KnowledgeMeaningId knowledgeMeaningId,
		KnowledgeUnitId knowledgeUnitId,
		VideoId videoId,
		string encounterSourcesJson,
		DateTimeOffset now)
	{
		var validation = Validate(userId, learningUnitId, knowledgeMeaningId, knowledgeUnitId, videoId);
		if (validation.IsFailure)
		{
			return Result.Failure<LearnerEncounteredLearningUnit>(validation.Error);
		}

		return Result.Success(new LearnerEncounteredLearningUnit(LearnerEncounteredLearningUnitId.Create())
		{
			UserId = userId,
			LearningUnitId = learningUnitId,
			KnowledgeMeaningId = knowledgeMeaningId,
			KnowledgeUnitId = knowledgeUnitId,
			VideoId = videoId,
			EncounterCount = 1,
			RawPlaybackCount = 1,
			EncounterSourcesJson = string.IsNullOrWhiteSpace(encounterSourcesJson) ? "[]" : encounterSourcesJson,
			FirstEncounteredAt = now,
			LastEncounteredAt = now
		});
	}

	private static Result Validate(
		UserId userId,
		LearningUnitId learningUnitId,
		KnowledgeMeaningId knowledgeMeaningId,
		KnowledgeUnitId knowledgeUnitId,
		VideoId videoId)
	{
		if (userId is null || userId.Value == Guid.Empty)
		{
			return Result.Failure(LearnerEncounteredLearningUnitErrors.InvalidUserId);
		}

		if (learningUnitId is null || learningUnitId.Value == Guid.Empty)
		{
			return Result.Failure(LearnerEncounteredLearningUnitErrors.InvalidLearningUnitId);
		}

		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
		{
			return Result.Failure(LearnerEncounteredLearningUnitErrors.InvalidMeaningId);
		}

		if (knowledgeUnitId is null || knowledgeUnitId.Value == Guid.Empty)
		{
			return Result.Failure(LearnerEncounteredLearningUnitErrors.InvalidKnowledgeUnitId);
		}

		if (videoId is null || videoId.Value == Guid.Empty)
		{
			return Result.Failure(LearnerEncounteredLearningUnitErrors.InvalidVideoId);
		}

		return Result.Success();
	}

	/// <summary>
	/// Records a subsequent encounter of this learning unit.
	/// Always increments RawPlaybackCount (+1), but only increments EncounterCount (+1) if the cooldown
	/// window since LastEncounteredAt has elapsed (Anti-Massed Repetition Contextual Capping).
	/// Returns true if EffectiveEncounterCount was incremented.
	/// </summary>
	public bool RecordEncounter(
		string? updatedEncounterSourcesJson,
		DateTimeOffset now,
		TimeSpan? customCooldown = null)
	{
		var cooldown = customCooldown ?? EffectiveEncounterCooldown;
		bool incrementedEffective = false;

		RawPlaybackCount += 1;

		if (now - LastEncounteredAt >= cooldown)
		{
			EncounterCount += 1;
			incrementedEffective = true;
		}

		if (!string.IsNullOrWhiteSpace(updatedEncounterSourcesJson))
		{
			EncounterSourcesJson = updatedEncounterSourcesJson;
		}

		LastEncounteredAt = now;
		return incrementedEffective;
	}
}
