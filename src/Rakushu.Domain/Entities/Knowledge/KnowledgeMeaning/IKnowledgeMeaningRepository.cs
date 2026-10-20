using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Material.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;

public interface IKnowledgeMeaningRepository : IBaseRepository<KnowledgeMeaning, KnowledgeMeaningId> 
{
	Task<IReadOnlyList<KnowledgeMeaning>> GetActiveMeaningsForEmbeddingAsync(
		int offset,
		int batchSize,
		CancellationToken cancellationToken = default);
}