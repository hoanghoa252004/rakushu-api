using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Persistence.Repositories;

public sealed class IssueDetailRepository : BaseRepository<IssueDetail, IssueDetailId>, IIssueDetailRepository
{
	public IssueDetailRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IssueDetail?> GetByIdWithReportAsync(
		IssueDetailId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Include(x => x.Report)
			.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
	}

	public async Task<bool> HasActiveIssueAsync(
		UserId userId,
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.AnyAsync(
				id => id.UserId == userId &&
					  id.TargetType == targetType &&
					  id.TargetId == targetId &&
					  id.Category == category &&
					  (id.ReportId == null || (id.Report != null && id.Report.Status != ReportStatus.Resolved && id.Report.Status != ReportStatus.Dismissed)),
				cancellationToken);
	}

	public async Task<IReadOnlyList<IssueDetail>> GetUnassignedByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Where(id => id.TargetType == targetType &&
						 id.TargetId == targetId &&
						 id.Category == category &&
						 id.ReportId == null)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<IssueDetail>> GetByUserIdAsync(
		UserId userId,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueDetails
			.Include(id => id.Report)
			.Where(id => id.UserId == userId)
			.OrderByDescending(id => id.CreatedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}
}
