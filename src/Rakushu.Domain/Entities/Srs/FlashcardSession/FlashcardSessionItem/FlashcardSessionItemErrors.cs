using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

public static class FlashcardSessionItemErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"FLASHCARD_SESSION_ITEM.NOT_FOUND",
		"Flashcard session item was not found.");
}
