using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;

public interface IKnowledgeUnitRepository : IBaseRepository<KnowledgeUnit, KnowledgeUnitId>
{
	Task<IReadOnlyList<KnowledgeUnit>> GetPagedAsync(
	int page,
	int pageSize,
	CancellationToken cancellationToken = default);

	Task<IReadOnlyList<KnowledgeUnit>> GetActiveByCollectionIdsAsync(
	IReadOnlyCollection<KnowledgeCollectionId> collectionIds,
	CancellationToken cancellationToken = default);
}
