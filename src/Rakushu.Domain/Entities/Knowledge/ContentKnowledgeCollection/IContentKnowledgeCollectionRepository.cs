using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.ContentCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.ContentKnowledgeCollection;

public interface IContentKnowledgeCollectionRepository : IBaseRepository<ContentKnowledgeCollection, ContentKnowledgeCollectionId>
{
	Task<IReadOnlyList<ContentKnowledgeCollection>> GetByContentCategoryIdAsync(
		ContentCategoryId contentCategoryId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyList<ContentKnowledgeCollection>>
	GetByContentCategoryIdOrderedByPriorityAsync(
		ContentCategoryId contentCategoryId,
		CancellationToken cancellationToken = default);
}
