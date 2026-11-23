using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;

namespace Rakushu.Application.Usecases.Curation.Curator.ResolveReport;

internal sealed class ResolveReportHandler : IRequestHandler<ResolveReportCommand, Result>
{
	private readonly IReportRepository _reportRepository;
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly IShadowingAttemptRepository _shadowingAttemptRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public ResolveReportHandler(
		IReportRepository reportRepository,
		IIssueCategoryRepository issueCategoryRepository,
		IShadowingAttemptRepository shadowingAttemptRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_reportRepository = reportRepository;
		_issueCategoryRepository = issueCategoryRepository;
		_shadowingAttemptRepository = shadowingAttemptRepository;
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

			if (report.TargetType == TargetType.ShadowingAttempt)
			{
				var assessments = await _shadowingAttemptRepository.GetAssessmentsWithDetailsAsync(
					ShadowingAttemptId.From(report.TargetId),
					cancellationToken);

				bool hasCuratorReevaluation = assessments.Any(a => a.Type == ShadowingAssessmentType.CuratorReevaluation);
				if (!hasCuratorReevaluation)
					return Result.Failure(ReportErrors.ShadowingReevaluationRequired);
			}

			var now = _systemClock.UtcNow;
			report.Resolve(request.CuratorNotes, now);

			var categoryAggregator = await _issueCategoryRepository.GetByTargetAndCategoryAsync(
				report.TargetType,
				report.TargetId,
				report.Category,
				report.StartCharIndex,
				report.EndCharIndex,
				cancellationToken);

			categoryAggregator?.ResetCount(now);

			return Result.Success();
		}, cancellationToken);
	}
}
