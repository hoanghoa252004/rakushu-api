using MediatR;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Material.Transcript;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingAttempt;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Application.Usecases.Curation.Curator.GetCuratorReports;

internal sealed class GetCuratorReportsHandler : IRequestHandler<GetCuratorReportsQuery, Result<PaginatedList<CuratorReportSummaryDto>>>
{
	private readonly IReportRepository _reportRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly ITranscriptRepository _transcriptRepository;
	private readonly ILearnerAnalyticsQueryService _learnerAnalyticsQueryService;

	public GetCuratorReportsHandler(
		IReportRepository reportRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		ITranscriptRepository transcriptRepository,
		ILearnerAnalyticsQueryService learnerAnalyticsQueryService)
	{
		_reportRepository = reportRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_transcriptRepository = transcriptRepository;
		_learnerAnalyticsQueryService = learnerAnalyticsQueryService;
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

			int? startCharIdx = r.StartCharIndex ?? snap?.StartCharIndex;
			int? endCharIdx = r.EndCharIndex ?? snap?.EndCharIndex;
			string? selectedOovText = snap?.SelectedOovText
				?? CurationContextHelper.ExtractOovText(snap?.OriginalSentenceJp, startCharIdx, endCharIdx);

			// Prioritize original reported snapshot context on Curator report list so Curator sees
			// what was originally reported; post-edit changes are inspected in Report Detail.
			string? targetTitle = CurationContextHelper.FormatSnapshotTitle(r.TargetType, snap);
			var targetPreview = CurationContextHelper.FormatSnapshotPreview(r.TargetType, r.Category, snap, selectedOovText);
			if (string.IsNullOrWhiteSpace(targetTitle) || targetPreview is null)
			{
				var (fallbackTitle, fallbackPreview) = await ResolveTargetContextAsync(
					r.TargetType,
					r.TargetId,
					r.Category,
					startCharIdx,
					endCharIdx,
					selectedOovText,
					cancellationToken);
				targetTitle ??= fallbackTitle;
				targetPreview ??= fallbackPreview;
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
				r.ResolvedAt,
				startCharIdx,
				endCharIdx,
				selectedOovText,
				snap?.VideoSourceType,
				targetPreview?.Headline,
				targetPreview?.Subline,
				targetPreview?.HighlightText,
				targetPreview?.IsHeadlineJapanese ?? false));
		}

		var pagedList = new PaginatedList<CuratorReportSummaryDto>(
			dtos,
			totalCount,
			page,
			pageSize);

		return Result.Success(pagedList);
	}

	private async Task<(string? TargetTitle, CuratorTargetPreviewDto? Preview)> ResolveTargetContextAsync(
		TargetType targetType,
		Guid targetId,
		string category,
		int? startCharIndex,
		int? endCharIndex,
		string? selectedOovText,
		CancellationToken cancellationToken)
	{
		try
		{
			switch (targetType)
			{
				case TargetType.BankQuestion:
				{
					var q = await _bankQuestionRepository.GetByIdWithDetailsAsync(
						BankQuestionId.From(targetId), cancellationToken);
					if (q is null)
					{
						return (null, null);
					}

					var plainQuestion = CurationContextHelper.StripHtmlToPlainText(q.Question);
					var expr = q.KnowledgeMeaning?.KnowledgeUnit?.Expression?.Trim();
					var meaning = q.KnowledgeMeaning?.Meaning?.Trim();
					string? subline = !string.IsNullOrWhiteSpace(expr)
						? !string.IsNullOrWhiteSpace(meaning) ? $"{expr} — {meaning}" : expr
						: null;

					return (
						$"Câu hỏi: {q.Question}",
						new CuratorTargetPreviewDto(
							string.IsNullOrWhiteSpace(plainQuestion) ? q.Question : plainQuestion,
							subline,
							null,
							false));
				}

				case TargetType.TranscriptSegment:
				{
					var seg = await _transcriptRepository.GetSegmentWithCurationContextByIdAsync(
						TranscriptSegmentId.From(targetId), cancellationToken);
					if (seg is null)
					{
						return (null, null);
					}

					var videoTitle = seg.Transcript?.Video?.Title ?? "Video bài học";
					var timestamp = CurationContextHelper.FormatTimestamp(seg.StartTime.TotalSeconds);
					var sentenceJp = seg.Text?.Trim();
					var isOov = string.Equals(category, CurationContextHelper.OovCategory, StringComparison.OrdinalIgnoreCase);
					var isWrongSpeaker = string.Equals(category, CurationContextHelper.WrongSpeakerCategory, StringComparison.OrdinalIgnoreCase);
					var oov = !string.IsNullOrWhiteSpace(selectedOovText)
						? selectedOovText.Trim()
						: CurationContextHelper.ExtractOovText(sentenceJp, startCharIndex, endCharIndex);
					var speakerName = !string.IsNullOrWhiteSpace(seg.TranscriptSpeaker?.Speaker?.Name)
						? seg.TranscriptSpeaker.Speaker.Name.Trim()
						: !string.IsNullOrWhiteSpace(seg.TranscriptSpeaker?.Label)
							? seg.TranscriptSpeaker.Label.Trim()
							: null;

					var headline = !string.IsNullOrWhiteSpace(sentenceJp) ? sentenceJp : videoTitle;
					var sublineParts = new List<string>(3);
					if (!string.IsNullOrWhiteSpace(sentenceJp))
					{
						sublineParts.Add(videoTitle);
					}

					sublineParts.Add(timestamp);
					if (isWrongSpeaker)
					{
						sublineParts.Add($"Vai: {speakerName ?? "Chưa gán vai"}");
					}

					return (
						$"Video: {videoTitle} ({timestamp})",
						new CuratorTargetPreviewDto(
							headline,
							string.Join(" • ", sublineParts),
							isOov && !string.IsNullOrWhiteSpace(sentenceJp) && !string.IsNullOrWhiteSpace(oov) ? oov : null,
							!string.IsNullOrWhiteSpace(sentenceJp)));
				}

				case TargetType.LearningUnit:
				{
					var (lu, _) = await _learningUnitRepository.GetCurationContextByIdAsync(
						LearningUnitId.From(targetId), cancellationToken);
					if (lu is null)
					{
						return (null, null);
					}

					var seg = lu.TranscriptSegment;
					var videoTitle = seg?.Transcript?.Video?.Title;
					var sentenceJp = seg?.Text?.Trim();
					var ts = seg is not null ? CurationContextHelper.FormatTimestamp(seg.StartTime.TotalSeconds) : null;
					var expr = lu.KnowledgeMeaning?.KnowledgeUnit?.Expression?.Trim();

					var title = !string.IsNullOrEmpty(videoTitle)
						? $"Video: {videoTitle} - Cụm 「{expr}」"
						: $"Đơn vị học: 「{expr}」";

					var headline = !string.IsNullOrWhiteSpace(sentenceJp)
						? sentenceJp
						: !string.IsNullOrWhiteSpace(expr)
							? expr
							: videoTitle;

					var sublineParts = new List<string>(3);
					if (!string.IsNullOrWhiteSpace(videoTitle) && !string.Equals(headline, videoTitle, StringComparison.Ordinal))
					{
						sublineParts.Add(videoTitle);
					}

					if (!string.IsNullOrWhiteSpace(ts))
					{
						sublineParts.Add(ts);
					}

					if (!string.IsNullOrWhiteSpace(expr) && !string.Equals(headline, expr, StringComparison.Ordinal))
					{
						sublineParts.Add(expr);
					}

					return (
						title,
						string.IsNullOrWhiteSpace(headline)
							? null
							: new CuratorTargetPreviewDto(
								headline,
								sublineParts.Count > 0 ? string.Join(" • ", sublineParts) : null,
								!string.IsNullOrWhiteSpace(sentenceJp) && !string.IsNullOrWhiteSpace(expr) ? expr : null,
								!string.IsNullOrWhiteSpace(sentenceJp) || !string.IsNullOrWhiteSpace(expr)));
				}

				case TargetType.ShadowingAttempt:
				{
					var attemptCtx = await _learnerAnalyticsQueryService.GetAttemptAssessmentsContextAsync(
						ShadowingAttemptId.From(targetId), cancellationToken);
					if (attemptCtx is null)
					{
						return (null, null);
					}

					var roleSuffix = !string.IsNullOrWhiteSpace(attemptCtx.RoleName)
						? $" ({attemptCtx.RoleName})"
						: string.Empty;
					var sublineParts = new List<string>(2);
					if (!string.IsNullOrWhiteSpace(attemptCtx.RoleName))
					{
						sublineParts.Add($"Vai: {attemptCtx.RoleName.Trim()}");
					}

					if (!string.IsNullOrWhiteSpace(attemptCtx.LearnerName))
					{
						sublineParts.Add(attemptCtx.LearnerName.Trim());
					}

					return (
						$"Shadowing: {attemptCtx.MaterialName}{roleSuffix}",
						new CuratorTargetPreviewDto(
							attemptCtx.MaterialName,
							sublineParts.Count > 0 ? string.Join(" • ", sublineParts) : null,
							null,
							false));
				}

				default:
					return (null, null);
			}
		}
		catch
		{
			return (null, null);
		}
	}
}
