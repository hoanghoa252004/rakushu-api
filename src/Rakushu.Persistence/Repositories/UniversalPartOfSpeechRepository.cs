using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech;

namespace Rakushu.Persistence.Repositories;

public sealed class UniversalPartOfSpeechRepository : BaseRepository<UniversalPartOfSpeech, UniversalPartOfSpeechId>, IUniversalPartOfSpeechRepository
{
	public UniversalPartOfSpeechRepository(RakushuDbContext context) : base(context) { }

	public async Task<UniversalPartOfSpeech?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
	{
		return await _context.UniversalPartOfSpeeches
			.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
	}
}
