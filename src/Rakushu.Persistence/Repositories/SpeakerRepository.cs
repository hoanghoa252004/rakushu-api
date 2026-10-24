using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Persistence.Repositories;

public sealed class SpeakerRepository : BaseRepository<Speaker, SpeakerId>, ISpeakerRepository
{
	public SpeakerRepository(RakushuDbContext context) : base(context) { }

	public async Task<bool> CodeExistsAsync(string code, SpeakerId? excludeId, CancellationToken cancellationToken = default)
	{
		return await _context.Speakers
			.AnyAsync(s => s.Code == code && (excludeId == null || s.Id != excludeId), cancellationToken);
	}

	public async Task<bool> IsUsedAsync(SpeakerId id, CancellationToken cancellationToken = default)
	{
		return await _context.TranscriptSpeakers
			.AnyAsync(ts => ts.SpeakerId == id, cancellationToken);
	}

	public async Task<(IReadOnlyList<Speaker> Items, int TotalCount)> GetPagedAsync(
		int pageNumber,
		int pageSize,
		string? searchTerm,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Speakers.AsNoTracking().AsQueryable();

		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var term = searchTerm.Trim().ToLower();
			query = query.Where(s =>
				s.Name.ToLower().Contains(term) ||
				s.JapaneseName.ToLower().Contains(term) ||
				s.Code.ToLower().Contains(term));
		}

		var total = await query.CountAsync(cancellationToken);
		var items = await query
			.OrderBy(s => s.Name)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, total);
	}

	public async Task<IReadOnlyList<Speaker>> GetByVideoIdAsync(VideoId videoId, CancellationToken cancellationToken = default)
	{
		var speakerIds = await _context.TranscriptSegments
			.Where(seg => seg.Transcript.VideoId == videoId && seg.TranscriptSpeaker.SpeakerId != null)
			.Select(seg => seg.TranscriptSpeaker.SpeakerId)
			.Distinct()
			.ToListAsync(cancellationToken);

		return await _context.Speakers
			.AsNoTracking()
			.Where(s => speakerIds.Contains(s.Id))
			.OrderBy(s => s.Name)
			.ToListAsync(cancellationToken);
	}
}
