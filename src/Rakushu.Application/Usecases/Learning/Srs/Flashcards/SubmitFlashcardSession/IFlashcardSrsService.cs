using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

public interface IFlashcardSrsService
{
	Task ApplySrsUpdatesAsync(
		FlashcardSession session,
		DateTimeOffset now,
		CancellationToken cancellationToken = default);
}
