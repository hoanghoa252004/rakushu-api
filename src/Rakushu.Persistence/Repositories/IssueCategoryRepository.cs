using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Persistence.Repositories;

public sealed class IssueCategoryRepository : BaseRepository<IssueCategory, IssueCategoryId>, IIssueCategoryRepository
{
	public IssueCategoryRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<IssueCategory?> GetByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default)
	{
		return await _context.IssueCategories
			.FirstOrDefaultAsync(
				ic => ic.TargetType == targetType && ic.TargetId == targetId && ic.Category == category,
				cancellationToken);
	}
}
