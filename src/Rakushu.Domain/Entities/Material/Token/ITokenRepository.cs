using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Knowledge.KnowledgeCollection;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Material.Token;

public interface ITokenRepository : IBaseRepository<Token, TokenId>
{
	Task<IReadOnlyList<Token>> GetByTranscriptSegmentIdAsync(TranscriptSegmentId transcriptSegmentId, CancellationToken cancellationToken = default);
}