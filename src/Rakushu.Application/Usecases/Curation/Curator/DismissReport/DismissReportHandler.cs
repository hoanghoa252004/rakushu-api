using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Curator.DismissReport;

internal sealed class DismissReportHandler : IRequestHandler<DismissReportCommand, Result>
{
	private readonly IReportRepository _reportRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DismissReportHandler(
		IReportRepository reportRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_reportRepository = reportRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		DismissReportCommand request,
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
			report.Dismiss(request.Reason, now);

			return Result.Success();
		}, cancellationToken);
	}
}
