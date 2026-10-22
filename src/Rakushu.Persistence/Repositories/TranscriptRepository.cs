using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Subscriptions.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Persistence.Repositories;
public sealed class TranscriptRepository : BaseRepository<Transcript, TranscriptId>, ITranscriptRepository
{
	public TranscriptRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Transcript?> GetByVideoIdAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		return await _context.Transcripts
			.Include(u => u.TranscriptSegments)
				.ThenInclude(u => u.Subtitle)
			.SingleOrDefaultAsync(u => u.VideoId == videoId, cancellationToken);
	}
}
