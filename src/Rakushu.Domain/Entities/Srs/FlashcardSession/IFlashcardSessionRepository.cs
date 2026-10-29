using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public interface IFlashcardSessionRepository : IBaseRepository<FlashcardSession, FlashcardSessionId>
{
	Task<FlashcardSession?> GetByIdWithItemsAsync(FlashcardSessionId id, CancellationToken cancellationToken = default);
	Task<FlashcardSession?> GetActiveSessionByUserAsync(UserId userId, CancellationToken cancellationToken = default);
}
