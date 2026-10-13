using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech;

namespace Rakushu.Persistence.Repositories;

public sealed class JapanesePartOfSpeechRepository : BaseRepository<JapanesePartOfSpeech, JapanesePartOfSpeechId>, IJapanesePartOfSpeechRepository
{
	public JapanesePartOfSpeechRepository(RakushuDbContext context) : base(context) { }

	public async Task<JapanesePartOfSpeech?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.JapanesePartOfSpeeches
			.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
	}
}
