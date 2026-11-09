using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionTarget;

public sealed class FlashcardSessionTarget
{
	public FlashcardSessionId FlashcardSessionId { get; private set; } = null!;
	public SavedItemId SavedItemId { get; private set; } = null!;

	// Navigation Properties
	public FlashcardSession FlashcardSession { get; private set; } = null!;
	public SavedItem.SavedItem SavedItem { get; private set; } = null!;

	private FlashcardSessionTarget() { }

	public static FlashcardSessionTarget Create(
		FlashcardSessionId sessionId,
		SavedItemId savedItemId)
	{
		return new FlashcardSessionTarget
		{
			FlashcardSessionId = sessionId,
			SavedItemId = savedItemId
		};
	}
}
