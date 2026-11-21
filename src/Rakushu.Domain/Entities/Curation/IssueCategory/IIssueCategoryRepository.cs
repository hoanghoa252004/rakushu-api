using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Domain.Entities.Curation.IssueCategory;

public interface IIssueCategoryRepository : IBaseRepository<IssueCategory, IssueCategoryId>
{
	Task<IssueCategory?> GetByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		int? startCharIndex = null,
		int? endCharIndex = null,
		CancellationToken cancellationToken = default);
}
