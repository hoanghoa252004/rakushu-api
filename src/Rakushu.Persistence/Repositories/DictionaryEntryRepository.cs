using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.DictionaryEntry;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class DictionaryEntryRepository : BaseRepository<DictionaryEntry, DictionaryEntryId>, IDictionaryEntryRepository
{
	public DictionaryEntryRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<DictionaryEntry?> GetByTermAsync(string term, CancellationToken cancellationToken = default)
	{
		return await _context.DictionaryEntries
			.FirstOrDefaultAsync(d => d.Term == term, cancellationToken);
	}
}
