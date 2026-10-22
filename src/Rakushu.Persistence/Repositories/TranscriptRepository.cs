using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Video;
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

	public async Task<Transcript?> GetByVideoIdAsync(
	VideoId videoId,
	int pageNumber,
	int pageSize,
	CancellationToken cancellationToken = default)
	{
		var skip = (pageNumber - 1) * pageSize;

		return await _context.Transcripts
			.AsNoTracking()
			.AsSplitQuery()

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.Subtitle)

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.LearningUnits)
					.ThenInclude(lu => lu.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
							.ThenInclude(ku => ku.ProficiencyLevel)

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.LearningUnits)
					.ThenInclude(lu => lu.KnowledgeMeaning)
						.ThenInclude(km => km.KnowledgeUnit)
							.ThenInclude(ku => ku.KnowledgeCollection)

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.TranscriptSpeaker)
					.ThenInclude(ts => ts.Speaker)

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.Bunsetsus)
					.ThenInclude(b => b.Tokens)
						.ThenInclude(token => token.JapanesePartOfSpeech)

			.Include(t => t.TranscriptSegments
				.OrderBy(s => s.Sequence)
				.Skip(skip)
				.Take(pageSize))
				.ThenInclude(s => s.Bunsetsus)
					.ThenInclude(b => b.Tokens)
						.ThenInclude(token => token.DependencyRelationship)

			.SingleOrDefaultAsync(
				t => t.VideoId == videoId,
				cancellationToken);
	}
}
