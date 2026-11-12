using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Persistence.Repositories;

public sealed class ReportRepository : BaseRepository<Report, ReportId>, IReportRepository
{
	public ReportRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Report?> GetByIdWithDetailsAsync(
		ReportId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.Reports
			.Include(r => r.Details)
				.ThenInclude(d => d.User)
			.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
	}

	public async Task<Report?> GetActiveReportByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.Reports
			.Include(r => r.Details)
			.FirstOrDefaultAsync(
				r => r.TargetType == targetType &&
					 r.TargetId == targetId &&
					 r.Category == category &&
					 (r.Status == ReportStatus.Pending || r.Status == ReportStatus.InReview),
				cancellationToken);
	}

	public async Task<(IReadOnlyList<Report> Items, int TotalCount)> GetReportsAsync(
		ReportStatus? statusFilter,
		TargetType? targetTypeFilter,
		string? categoryFilter,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.Reports
			.Include(r => r.Details)
			.AsQueryable();

		if (statusFilter.HasValue)
		{
			query = query.Where(r => r.Status == statusFilter.Value);
		}

		if (targetTypeFilter.HasValue)
		{
			query = query.Where(r => r.TargetType == targetTypeFilter.Value);
		}

		if (!string.IsNullOrWhiteSpace(categoryFilter))
		{
			query = query.Where(r => r.Category == categoryFilter);
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(r => r.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
	}
}
