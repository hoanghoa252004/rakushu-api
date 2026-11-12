using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Curator.ResolveReport;

internal sealed class ResolveReportHandler : IRequestHandler<ResolveReportCommand, Result>
{
	private readonly IReportRepository _reportRepository;
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ResolveReportHandler(
		IReportRepository reportRepository,
		IIssueCategoryRepository issueCategoryRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_reportRepository = reportRepository;
		_issueCategoryRepository = issueCategoryRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		ResolveReportCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var reportId = ReportId.From(request.ReportId);
			var report = await _reportRepository.GetByIdAsync(reportId, cancellationToken);
			if (report is null)
				return Result.Failure(ReportErrors.NotFound);

			if (report.Status == ReportStatus.Resolved)
				return Result.Failure(ReportErrors.AlreadyResolved);

			if (report.Status == ReportStatus.Dismissed)
				return Result.Failure(ReportErrors.AlreadyDismissed);

			var now = _systemClock.UtcNow;
			report.Resolve(request.CuratorNotes, now);

			var categoryAggregator = await _issueCategoryRepository.GetByTargetAndCategoryAsync(
				report.TargetType,
				report.TargetId,
				report.Category,
				cancellationToken);

			categoryAggregator?.ResetCount(now);

			return Result.Success();
		}, cancellationToken);
	}
}
