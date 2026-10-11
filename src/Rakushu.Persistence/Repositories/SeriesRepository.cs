using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Series;

namespace Rakushu.Persistence.Repositories;

public sealed class SeriesRepository : BaseRepository<Series, SeriesId>, ISeriesRepository
{
	public SeriesRepository(RakushuDbContext context) : base(context) { }

	public override async Task<Series?> GetByIdAsync(SeriesId id, CancellationToken cancellationToken = default)
	{
		return await _context.Series
			.Include(s => s.Videos)
			.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
	}

	public async Task<Series?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Series
			.Include(s => s.Videos)
			.FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken);
	}

	public override async Task<IEnumerable<Series>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		return await _context.Series
			.Include(s => s.Videos)
			.ToListAsync(cancellationToken);
	}
}
