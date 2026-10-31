using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Persistence.Repositories;

public sealed class FlashcardSessionRepository : BaseRepository<FlashcardSession, FlashcardSessionId>, IFlashcardSessionRepository
{
	public FlashcardSessionRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<FlashcardSession?> GetByIdWithItemsAsync(
		FlashcardSessionId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
			.SingleOrDefaultAsync(fs => fs.Id == id, cancellationToken);
	}

	public async Task<FlashcardSession?> GetActiveSessionByUserAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.FlashcardSessions
			.Include(fs => fs.Items)
				.ThenInclude(i => i.SavedItem)
			.FirstOrDefaultAsync(
				fs => fs.UserId == userId && fs.Status == FlashcardSessionStatus.InProgress,
				cancellationToken);
	}
}
