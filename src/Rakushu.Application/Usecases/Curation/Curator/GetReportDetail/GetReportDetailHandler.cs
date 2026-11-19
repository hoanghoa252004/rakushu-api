using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Curator.GetReportDetail;

internal sealed class GetReportDetailHandler : IRequestHandler<GetReportDetailQuery, Result<CuratorReportDetailDto>>
{
	private readonly IReportRepository _reportRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly IStorageService _storageService;

	public GetReportDetailHandler(
		IReportRepository reportRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		IStorageService storageService)
	{
		_reportRepository = reportRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_storageService = storageService;
	}

	public async Task<Result<CuratorReportDetailDto>> Handle(
		GetReportDetailQuery request,
		CancellationToken cancellationToken)
	{
		var reportId = ReportId.From(request.ReportId);
		var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
		if (report is null)
		{
			return Result.Failure<CuratorReportDetailDto>(ReportErrors.NotFound);
		}

		var feedbacks = report.Details.Select(d => new IssueFeedbackDto(
			d.UserId.Value,
			d.User?.FullName,
			d.LearnerFeedback,
			d.CreatedAt)).ToList();

		BankQuestionContextDto? questionContext = null;
		VideoLearningUnitContextDto? videoContext = null;

		try
		{
			if (report.TargetType == TargetType.BankQuestion)
			{
				var q = await _bankQuestionRepository.GetByIdWithDetailsAsync(
					BankQuestionId.From(report.TargetId), cancellationToken);
				if (q is not null)
				{
					questionContext = new BankQuestionContextDto(
						q.Id.Value,
						q.Question,
						q.QuestionType?.Code ?? "SINGLE_CHOICE",
						q.Answers,
						q.CorrectAnswer,
						q.Explanation,
						q.KnowledgeMeaning?.KnowledgeUnit?.Expression,
						q.KnowledgeMeaning?.KnowledgeUnit?.Reading,
						q.KnowledgeMeaning?.Meaning,
						q.IsActive);
				}
			}
			else if (report.TargetType == TargetType.TranscriptSegment)
			{
				var segment = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
					TranscriptSegmentId.From(report.TargetId), cancellationToken);
				if (segment is not null)
				{
					var video = segment.Transcript?.Video;
					double startTimeSecs = segment.StartTime.TotalSeconds;
					double endTimeSecs = segment.EndTime.TotalSeconds;
					double durationSecs = video?.Duration.TotalSeconds ?? 0;

					var (videoUrl, thumbnailUrl) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
						video,
						_storageService,
						includeThumbnail: true,
						cancellationToken);

					videoContext = new VideoLearningUnitContextDto(
						null,
						video?.Id.Value ?? Guid.Empty,
						video?.Title ?? "Video bài học",
						videoUrl,
						thumbnailUrl,
						durationSecs,
						CurationContextHelper.FormatTimestamp(durationSecs),
						startTimeSecs,
						endTimeSecs,
						startTimeSecs,
						CurationContextHelper.FormatTimestamp(startTimeSecs),
						segment.Text ?? string.Empty,
						segment.Subtitle?.Text,
						null,
						0,
						0,
						null,
						null);
				}
			}
			else if (report.TargetType == TargetType.LearningUnit)
			{
				var (unit, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
					LearningUnitId.From(report.TargetId), cancellationToken);
				if (unit is not null)
				{
					var segment = unit.TranscriptSegment;
					var video = segment?.Transcript?.Video;
					double startTimeSecs = segment?.StartTime.TotalSeconds ?? 0;
					double endTimeSecs = segment?.EndTime.TotalSeconds ?? 0;
					double durationSecs = video?.Duration.TotalSeconds ?? 0;

					var (videoUrl, thumbnailUrl) = await CurationContextHelper.ResolveVideoMediaUrlsAsync(
						video,
						_storageService,
						includeThumbnail: true,
						cancellationToken);

					var targetToken = unit.KnowledgeMeaning?.KnowledgeUnit?.Expression;

					videoContext = new VideoLearningUnitContextDto(
						unit.Id.Value,
						video?.Id.Value ?? Guid.Empty,
						video?.Title ?? "Video bài học",
						videoUrl,
						thumbnailUrl,
						durationSecs,
						CurationContextHelper.FormatTimestamp(durationSecs),
						startTimeSecs,
						endTimeSecs,
						startTimeSecs,
						CurationContextHelper.FormatTimestamp(startTimeSecs),
						segment?.Text ?? string.Empty,
						segment?.Subtitle?.Text,
						targetToken,
						unit.StartTokenIndex,
						unit.EndTokenIndex,
						targetToken,
						unit.KnowledgeMeaning?.Meaning);
				}
			}
		}
		catch
		{
			// Non-critical: Context enrichment failure should not block fetching the report ticket
		}

		var context = (questionContext is not null || videoContext is not null)
			? new ReportTargetContextDto(questionContext, videoContext)
			: null;

		var rawSnapshotJson = report.TargetSnapshot
			?? report.Details
				.OrderBy(d => d.CreatedAt)
				.Select(d => d.TargetSnapshot)
				.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
		var reportedSnapshot = CurationContextHelper.DeserializeSnapshot(rawSnapshotJson);

		var dto = new CuratorReportDetailDto(
			report.Id.Value,
			report.TargetType.ToString(),
			report.TargetId,
			report.Category,
			report.Status.ToString(),
			report.CuratorNotes,
			report.CreatedAt,
			report.ResolvedAt,
			feedbacks,
			context,
			reportedSnapshot);

		return Result.Success(dto);
	}
}
