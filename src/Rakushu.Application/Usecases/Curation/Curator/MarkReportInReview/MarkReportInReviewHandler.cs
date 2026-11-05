using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Curator.MarkReportInReview;

internal sealed class MarkReportInReviewHandler : IRequestHandler<MarkReportInReviewCommand, Result>
{
	private readonly IReportRepository _reportRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public MarkReportInReviewHandler(
		IReportRepository reportRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_reportRepository = reportRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		MarkReportInReviewCommand request,
		CancellationToken cancellationToken)
	{
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var reportId = ReportId.From(request.ReportId);
			var report = await _reportRepository.GetByIdAsync(reportId, cancellationToken);
			if (report is null)
				return Result.Failure(ReportErrors.NotFound);

			if (report.Status != ReportStatus.Pending)
				return Result.Failure(ReportErrors.InvalidStatusTransition);

			report.MarkInReview(_systemClock.UtcNow);

			return Result.Success();
		}, cancellationToken);
	}
}
