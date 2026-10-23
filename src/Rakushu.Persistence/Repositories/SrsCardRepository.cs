using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class SrsCardRepository : BaseRepository<SrsCard, SrsCardId>, ISrsCardRepository
{
	public SrsCardRepository(RakushuDbContext context) : base(context) { }

	public async Task<IReadOnlyList<SrsCard>> GetCardsAsync(
		UserId userId, CancellationToken cancellationToken = default)
	{
		return await QueryQuizCards(userId, DateTimeOffset.MinValue, false)
			.AsNoTracking()
			.OrderBy(card => card.State!.DueAt)
			.ThenBy(card => card.CreatedAt)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<SrsCard>> GetQuizCardsByIdsAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		DateTimeOffset now,
		bool dueOnly,
		CancellationToken cancellationToken = default)
	{
		if (cardIds.Count == 0)
		{
			return Array.Empty<SrsCard>();
		}

		// Compare the mapped strongly typed ID itself so EF Core can apply its
		// SrsCardId <-> Guid value converter when translating Contains().
		var typedCardIds = cardIds.Select(SrsCardId.From).ToArray();
		var query = QueryQuizCards(userId, now, dueOnly)
			.Where(card => typedCardIds.Contains(card.Id));

		return await query
			.AsNoTracking()
			.ToListAsync(cancellationToken);
	}

	private IQueryable<SrsCard> QueryQuizCards(UserId userId, DateTimeOffset now, bool dueOnly)
	{
		var query = _context.SrsCards
			.Include(card => card.State)
			.Include(card => card.SrsNote)
				.ThenInclude(note => note.LearningUnit)
					.ThenInclude(unit => unit.KnowledgeMeaning)
						.ThenInclude(meaning => meaning.KnowledgeUnit)
			.Where(card =>
				card.SrsNote.UserId == userId &&
				card.SrsNote.ArchivedAt == null &&
				card.State != null &&
				card.Status != SrsCardStatus.Suspended &&
				card.Status != SrsCardStatus.Archived);

		if (dueOnly)
		{
			query = query.Where(card => card.State!.DueAt <= now);
		}

		return query;
	}

	public void AddState(SrsCardState state) => _context.SrsCardStates.Add(state);
	public void AddReviewLog(SrsReviewLog log) => _context.SrsReviewLogs.Add(log);
}
