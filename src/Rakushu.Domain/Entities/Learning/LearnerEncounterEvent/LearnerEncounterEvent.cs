using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Domain.Entities.Learning.LearnerEncounterEvent;

/// <summary>
/// Append-only event log recording each effective (cooldown-verified) encounter of a LearningUnit
/// during video playback. Enables accurate sliding-window counts (e.g., 7-day immersion window)
/// and per-schedule watermark filtering without over-counting historical encounters.
/// </summary>
public sealed class LearnerEncounterEvent : Entity<LearnerEncounterEventId>
{
	public UserId UserId { get; private set; } = null!;
	public LearningUnitId LearningUnitId { get; private set; } = null!;
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public VideoId VideoId { get; private set; } = null!;
	public DateTimeOffset EncounteredAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public LearningUnit LearningUnit { get; private set; } = null!;
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public Video Video { get; private set; } = null!;

	private LearnerEncounterEvent()
	{
	}

	private LearnerEncounterEvent(LearnerEncounterEventId id) : base(id)
	{
	}

	public static Result<LearnerEncounterEvent> Create(
		UserId userId,
		LearningUnitId learningUnitId,
		KnowledgeMeaningId knowledgeMeaningId,
		VideoId videoId,
		DateTimeOffset encounteredAt)
	{
		if (userId is null || userId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerEncounterEvent>(LearnerEncounterEventErrors.InvalidUserId);
		}

		if (learningUnitId is null || learningUnitId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerEncounterEvent>(LearnerEncounterEventErrors.InvalidLearningUnitId);
		}

		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerEncounterEvent>(LearnerEncounterEventErrors.InvalidMeaningId);
		}

		if (videoId is null || videoId.Value == Guid.Empty)
		{
			return Result.Failure<LearnerEncounterEvent>(LearnerEncounterEventErrors.InvalidVideoId);
		}

		return Result.Success(new LearnerEncounterEvent(LearnerEncounterEventId.Create())
		{
			UserId = userId,
			LearningUnitId = learningUnitId,
			KnowledgeMeaningId = knowledgeMeaningId,
			VideoId = videoId,
			EncounteredAt = encounteredAt
		});
	}
}
