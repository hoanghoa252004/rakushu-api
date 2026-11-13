using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionCard;

public sealed class FlashcardSessionCard
{
	public FlashcardSessionId FlashcardSessionId { get; private set; } = null!;
	public SavedItemId SavedItemId { get; private set; } = null!;

	// Navigation Properties
	public FlashcardSession FlashcardSession { get; private set; } = null!;
	public SavedItem.SavedItem SavedItem { get; private set; } = null!;

	private FlashcardSessionCard() { }

	public static FlashcardSessionCard Create(
		FlashcardSessionId sessionId,
		SavedItemId savedItemId)
	{
		return new FlashcardSessionCard
		{
			FlashcardSessionId = sessionId,
			SavedItemId = savedItemId
		};
	}
}
