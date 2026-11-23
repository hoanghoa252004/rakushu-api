using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Learning.LearnerVideoProgress;
using Rakushu.Domain.Entities.Material.Video;

namespace Rakushu.Persistence.Repositories;

public sealed class LearnerVideoProgressRepository
	: BaseRepository<LearnerVideoProgress, LearnerVideoProgressId>, ILearnerVideoProgressRepository
{
	public LearnerVideoProgressRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<LearnerVideoProgress?> GetByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		return await _context.LearnerVideoProgresses
			.FirstOrDefaultAsync(
				x => x.UserId == userId && x.VideoId == videoId,
				cancellationToken);
	}

	public async Task<IReadOnlyList<LearnerVideoProgress>> GetByUserWithVideoDetailsAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.LearnerVideoProgresses
			.AsNoTracking()
			.Include(x => x.Video)
				.ThenInclude(v => v.MediaAssets)
			.Include(x => x.Video)
				.ThenInclude(v => v.Series)
			.Include(x => x.Video)
				.ThenInclude(v => v.ContentCategory)
			.Include(x => x.Video)
				.ThenInclude(v => v.Transcript)
					.ThenInclude(t => t.TranscriptSegments)
						.ThenInclude(ts => ts.LearningUnits)
							.ThenInclude(lu => lu.KnowledgeMeaning)
								.ThenInclude(km => km.KnowledgeUnit)
									.ThenInclude(ku => ku.ProficiencyLevel)
			.Where(x => x.UserId == userId)
			.OrderByDescending(x => x.LastWatchedAt)
			.AsSplitQuery()
			.ToListAsync(cancellationToken);
	}

	public async Task<bool> DeleteByUserAndVideoAsync(
		UserId userId,
		VideoId videoId,
		CancellationToken cancellationToken = default)
	{
		var existing = await _context.LearnerVideoProgresses
			.FirstOrDefaultAsync(
				x => x.UserId == userId && x.VideoId == videoId,
				cancellationToken);

		if (existing is null)
		{
			return false;
		}

		_context.LearnerVideoProgresses.Remove(existing);
		return true;
	}
}
