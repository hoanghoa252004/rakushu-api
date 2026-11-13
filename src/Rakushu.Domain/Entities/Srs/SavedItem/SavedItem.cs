using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public sealed class SavedItem : AggregateRoot<SavedItemId>
{
	public UserId UserId { get; private set; } = null!;
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;

	// Live vocabulary data dynamically accessed from Knowledge module
	public string Expression => KnowledgeMeaning?.KnowledgeUnit?.Expression ?? KnowledgeMeaning?.Meaning ?? string.Empty;
	public string? Reading => KnowledgeMeaning?.KnowledgeUnit?.Reading;
	public string Meaning => KnowledgeMeaning?.Meaning ?? string.Empty;

	public bool IsActive { get; private set; } = true;
	public DateTimeOffset SavedAt { get; private set; }

	// Automatically derived: true if the underlying meaning has been revoked/deactivated by curators
	public bool IsRevoked => KnowledgeMeaning is not null && !KnowledgeMeaning.IsActive;

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public SrsSchedule.SrsSchedule? Schedule { get; private set; }

	private SavedItem() { }
	private SavedItem(SavedItemId id) : base(id) { }

	public static Result<SavedItem> Create(
		UserId userId,
		KnowledgeMeaningId knowledgeMeaningId,
		DateTimeOffset now,
		bool isActive = true)
	{
		var validation = Validate(userId, knowledgeMeaningId);
		if (validation.IsFailure)
			return Result.Failure<SavedItem>(validation.Error);

		return Result.Success(new SavedItem(SavedItemId.Create())
		{
			UserId = userId,
			KnowledgeMeaningId = knowledgeMeaningId,
			IsActive = isActive,
			SavedAt = now
		});
	}

	public void AttachSchedule(SrsSchedule.SrsSchedule schedule)
	{
		Schedule = schedule;
	}

	public void ToggleActive(bool isActive)
	{
		IsActive = isActive;
	}

	public void Activate()
	{
		IsActive = true;
	}

	public void Deactivate()
	{
		IsActive = false;
	}

	private static Result Validate(
		UserId userId,
		KnowledgeMeaningId knowledgeMeaningId)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure(SavedItemErrors.InvalidUserId);

		if (knowledgeMeaningId is null || knowledgeMeaningId.Value == Guid.Empty)
			return Result.Failure(SavedItemErrors.InvalidMeaningId);

		return Result.Success();
	}
}
