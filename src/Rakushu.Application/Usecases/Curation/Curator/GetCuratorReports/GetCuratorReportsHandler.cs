using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Curator.GetCuratorReports;

internal sealed class GetCuratorReportsHandler : IRequestHandler<GetCuratorReportsQuery, Result<PaginatedList<CuratorReportSummaryDto>>>
{
	private readonly IReportRepository _reportRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;

	public GetCuratorReportsHandler(
		IReportRepository reportRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository)
	{
		_reportRepository = reportRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
	}

	public async Task<Result<PaginatedList<CuratorReportSummaryDto>>> Handle(
		GetCuratorReportsQuery request,
		CancellationToken cancellationToken)
	{
		ReportStatus? status = null;
		if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<ReportStatus>(request.Status, true, out var parsedStatus))
		{
			status = parsedStatus;
		}

		TargetType? targetType = null;
		if (!string.IsNullOrWhiteSpace(request.TargetType) && Enum.TryParse<TargetType>(request.TargetType, true, out var parsedTargetType))
		{
			targetType = parsedTargetType;
		}

		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var (items, totalCount) = await _reportRepository.GetReportsAsync(
			status,
			targetType,
			request.Category,
			page,
			pageSize,
			cancellationToken);

		var dtos = new List<CuratorReportSummaryDto>(items.Count);
		foreach (var r in items)
		{
			var snapshotJson = r.TargetSnapshot
				?? r.Details
					.OrderBy(d => d.CreatedAt)
					.Select(d => d.TargetSnapshot)
					.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
			var snap = CurationContextHelper.DeserializeSnapshot(snapshotJson);

			// Prioritize original reported snapshot title on Curator report list so Curator sees
			// what was originally reported; post-edit changes are inspected in Report Detail.
			string? targetTitle = CurationContextHelper.FormatSnapshotTitle(r.TargetType, snap);
			if (string.IsNullOrWhiteSpace(targetTitle))
			{
				targetTitle = await ResolveTargetTitleAsync(r.TargetType, r.TargetId, cancellationToken);
			}

			dtos.Add(new CuratorReportSummaryDto(
				r.Id.Value,
				r.TargetType.ToString(),
				r.TargetId,
				targetTitle,
				r.Category,
				r.Status.ToString(),
				r.Details.Count,
				r.CreatedAt,
				r.ResolvedAt));
		}

		var pagedList = new PaginatedList<CuratorReportSummaryDto>(
			dtos,
			totalCount,
			page,
			pageSize);

		return Result.Success(pagedList);
	}

	private async Task<string?> ResolveTargetTitleAsync(
		TargetType targetType,
		Guid targetId,
		CancellationToken cancellationToken)
	{
		try
		{
			switch (targetType)
			{
				case TargetType.BankQuestion:
					var q = await _bankQuestionRepository.GetByIdWithDetailsAsync(
						BankQuestionId.From(targetId), cancellationToken);
					return q is not null ? $"Câu hỏi: {q.Question}" : null;

				case TargetType.TranscriptSegment:
					var seg = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
						TranscriptSegmentId.From(targetId), cancellationToken);
					if (seg is not null)
					{
						var videoTitle = seg.Transcript?.Video?.Title ?? "Video bài học";
						var timestamp = CurationContextHelper.FormatTimestamp(seg.StartTime.TotalSeconds);
						var preview = seg.Text.Length > 35 ? seg.Text[..35] + "..." : seg.Text;
						return $"Video: {videoTitle} ({timestamp}) - 「{preview}」";
					}
					return null;

				case TargetType.LearningUnit:
					var (lu, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
						LearningUnitId.From(targetId), cancellationToken);
					if (lu is not null)
					{
						var videoTitle = lu.TranscriptSegment?.Transcript?.Video?.Title;
						var expr = lu.KnowledgeMeaning?.KnowledgeUnit?.Expression;
						return !string.IsNullOrEmpty(videoTitle)
							? $"Video: {videoTitle} - Cụm 「{expr}」"
							: $"Đơn vị học: 「{expr}」";
					}
					return null;

				default:
					return null;
			}
		}
		catch
		{
			return null;
		}
	}
}
