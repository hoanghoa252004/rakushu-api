using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Curation.Report;

public interface IReportRepository : IBaseRepository<Report, ReportId>
{
	Task<Report?> GetByIdWithDetailsAsync(
		ReportId id,
		CancellationToken cancellationToken = default);

	Task<Report?> GetActiveReportByTargetAndCategoryAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<Report> Items, int TotalCount)> GetReportsAsync(
		ReportStatus? statusFilter,
		TargetType? targetTypeFilter,
		string? categoryFilter,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);
}
