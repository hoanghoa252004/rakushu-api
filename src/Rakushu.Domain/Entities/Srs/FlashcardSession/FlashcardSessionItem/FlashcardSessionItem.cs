using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

public sealed class FlashcardSessionItem : Entity<FlashcardSessionItemId>
{
	public FlashcardSessionId FlashcardSessionId { get; private set; } = null!;
	public SavedItemId SavedItemId { get; private set; } = null!;

	// Rating result
	public FlashcardRating? Rating { get; private set; }
	public DateTimeOffset? ReviewedAt { get; private set; }

	// Navigation Properties
	public FlashcardSession FlashcardSession { get; private set; } = null!;
	public SavedItem.SavedItem SavedItem { get; private set; } = null!;

	private FlashcardSessionItem() { }
	private FlashcardSessionItem(FlashcardSessionItemId id) : base(id) { }

	public static Result<FlashcardSessionItem> Create(
		FlashcardSessionId sessionId,
		SavedItemId savedItemId)
	{
		return Result.Success(new FlashcardSessionItem(FlashcardSessionItemId.Create())
		{
			FlashcardSessionId = sessionId,
			SavedItemId = savedItemId
		});
	}

	public void RecordRating(FlashcardRating rating, DateTimeOffset reviewedAt)
	{
		Rating = rating;
		ReviewedAt = reviewedAt;
	}
}
