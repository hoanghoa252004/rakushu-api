using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

public sealed class FlashcardSessionItem : Entity<FlashcardSessionItemId>
{
	public FlashcardSessionId FlashcardSessionId { get; private set; } = null!;
	public SavedItemId SavedItemId { get; private set; } = null!;

	// Vocabulary Snapshot
	public string Expression { get; private set; } = null!;
	public string? Reading { get; private set; }
	public string Meaning { get; private set; } = null!;

	// Multimedia Context Snapshot
	public string? VideoTitle { get; private set; }
	public string? StorageKey { get; private set; }
	public string? VideoSourceUrl { get; private set; }
	public TimeSpan? StartTime { get; private set; }
	public TimeSpan? EndTime { get; private set; }
	public string? OriginalSentence { get; private set; }
	public string? TranslatedSentence { get; private set; }
	public int? StartTokenIndex { get; private set; }
	public int? EndTokenIndex { get; private set; }

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
		SavedItemId savedItemId,
		string expression,
		string? reading,
		string meaning,
		string? videoTitle,
		string? storageKey,
		string? videoSourceUrl,
		TimeSpan? startTime,
		TimeSpan? endTime,
		string? originalSentence,
		string? translatedSentence,
		int? startTokenIndex,
		int? endTokenIndex)
	{
		return Result.Success(new FlashcardSessionItem(FlashcardSessionItemId.Create())
		{
			FlashcardSessionId = sessionId,
			SavedItemId = savedItemId,
			Expression = expression,
			Reading = reading,
			Meaning = meaning,
			VideoTitle = videoTitle,
			StorageKey = storageKey,
			VideoSourceUrl = videoSourceUrl,
			StartTime = startTime,
			EndTime = endTime,
			OriginalSentence = originalSentence,
			TranslatedSentence = translatedSentence,
			StartTokenIndex = startTokenIndex,
			EndTokenIndex = endTokenIndex
		});
	}

	public void RecordRating(FlashcardRating rating, DateTimeOffset reviewedAt)
	{
		Rating = rating;
		ReviewedAt = reviewedAt;
	}
}
