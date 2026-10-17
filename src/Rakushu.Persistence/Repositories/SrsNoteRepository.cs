using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class SrsNoteRepository : BaseRepository<SrsNote, SrsNoteId>, ISrsNoteRepository
{
	public SrsNoteRepository(RakushuDbContext context) : base(context) { }

	public Task<bool> LearningUnitExistsAsync(LearningUnitId learningUnitId, CancellationToken cancellationToken = default) =>
		_context.LearningUnits.AnyAsync(unit => unit.Id == learningUnitId, cancellationToken);

	public async Task<(SrsNote? Note, SrsCard? Card)> GetEnrollmentAsync(UserId userId, LearningUnitId learningUnitId, CancellationToken cancellationToken = default)
	{
		var note = await _context.SrsNotes.FirstOrDefaultAsync(
			item => item.UserId == userId && item.LearningUnitId == learningUnitId, cancellationToken);
		if (note is null)
			return (null, null);

		var card = await _context.SrsCards.Include(item => item.State)
			.FirstOrDefaultAsync(item => item.SrsNoteId == note.Id, cancellationToken);
		return (note, card);
	}
}