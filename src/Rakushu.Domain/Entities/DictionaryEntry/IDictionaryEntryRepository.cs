using Rakushu.Domain.Common.Contract;
using System.Threading;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.DictionaryEntry;

public interface IDictionaryEntryRepository : IBaseRepository<DictionaryEntry, DictionaryEntryId>
{
	Task<DictionaryEntry?> GetByTermAsync(string term, CancellationToken cancellationToken = default);
}
