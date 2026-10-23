using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Token;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;

public sealed class TokenRepository : BaseRepository<Token, TokenId>, ITokenRepository
{
	public TokenRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IReadOnlyList<Token>> GetByTranscriptSegmentIdAsync(TranscriptSegmentId transcriptSegmentId, CancellationToken cancellationToken = default)
	{
		return await _context.Tokens
			.Include(t => t.Bunsetsu)
				.ThenInclude(b => b.TranscriptSegment)
			.Where(t => t.Bunsetsu.TranscriptSegmentId == transcriptSegmentId)
			.ToListAsync(cancellationToken);
	}
}