using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.IssueCategory;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAssessment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Curator.DismissReport;

internal sealed class DismissReportHandler : IRequestHandler<DismissReportCommand, Result>
{
	private readonly IReportRepository _reportRepository;
	private readonly IIssueCategoryRepository _issueCategoryRepository;
	private readonly IShadowingAttemptRepository _shadowingAttemptRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public DismissReportHandler(
		IReportRepository reportRepository,
		IIssueCategoryRepository issueCategoryRepository,
		IShadowingAttemptRepository shadowingAttemptRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_reportRepository = reportRepository;
		_issueCategoryRepository = issueCategoryRepository;
		_shadowingAttemptRepository = shadowingAttemptRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
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
			var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
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
				if (hasCuratorReevaluation)
					return Result.Failure(ReportErrors.ShadowingAlreadyReevaluatedCannotDismiss);
			}
			else if (report.TargetType == TargetType.BankQuestion)
			{
				var snapshot = CurationContextHelper.ResolveCanonicalSnapshot(report);
				var liveQuestion = await _bankQuestionRepository.GetByIdWithDetailsAsync(
					BankQuestionId.From(report.TargetId),
					cancellationToken);

				if (CurationContextHelper.HasBankQuestionChangedOrArchived(report, snapshot, liveQuestion))
					return Result.Failure(ReportErrors.BankQuestionAlreadyUpdatedCannotDismiss);
			}
			else if (report.TargetType == TargetType.TranscriptSegment)
			{
				var snapshot = CurationContextHelper.ResolveCanonicalSnapshot(report);
				var liveSegment = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
					TranscriptSegmentId.From(report.TargetId),
					cancellationToken);

				bool isWrongSpeaker = string.Equals(
					report.Category,
					CurationContextHelper.WrongSpeakerCategory,
					StringComparison.OrdinalIgnoreCase);

				if (isWrongSpeaker
					&& CurationContextHelper.HasSegmentSpeakerChanged(snapshot, liveSegment, includeUnassignedToAssigned: true))
					return Result.Failure(ReportErrors.WrongSpeakerAlreadyUpdatedCannotDismiss);

				if (CurationContextHelper.HasTranscriptSegmentChangedOrRemoved(report, snapshot, liveSegment))
					return Result.Failure(ReportErrors.TranscriptSegmentAlreadyUpdatedCannotDismiss);
			}
			else if (report.TargetType == TargetType.LearningUnit)
			{
				var snapshot = CurationContextHelper.ResolveCanonicalSnapshot(report);
				var (liveUnit, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
					LearningUnitId.From(report.TargetId),
					cancellationToken);

				if (CurationContextHelper.HasLearningUnitChangedOrRemoved(snapshot, liveUnit))
					return Result.Failure(ReportErrors.LearningUnitAlreadyUpdatedCannotDismiss);
			}

			var now = _systemClock.UtcNow;
			report.Dismiss(request.Reason, now);

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
