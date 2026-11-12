using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Domain.Entities.Curation.IssueCategory;

public interface IIssueCategoryRepository : IBaseRepository<IssueCategory, IssueCategoryId>
{
	Task<IssueCategory?> GetByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default);
}
