using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;

namespace Rakushu.Application.Usecases.Curation.Curator.GetOovStats;

internal sealed class GetOovStatsHandler : IRequestHandler<GetOovStatsQuery, Result<PaginatedList<CuratorOovStatDto>>>
{
	private readonly IIssueDetailRepository _issueDetailRepository;

	public GetOovStatsHandler(IIssueDetailRepository issueDetailRepository)
	{
		_issueDetailRepository = issueDetailRepository;
	}

	public async Task<Result<PaginatedList<CuratorOovStatDto>>> Handle(
		GetOovStatsQuery request,
		CancellationToken cancellationToken)
	{
		int page = Math.Max(1, request.Page);
		int pageSize = Math.Clamp(request.PageSize, 1, 100);

		var (rows, totalCount) = await _issueDetailRepository.GetCuratedOovStatsAsync(
			request.VideoId,
			request.Status,
			request.Search,
			page,
			pageSize,
			cancellationToken);

		var dtos = rows.Select(row =>
		{
			var snap = CurationContextHelper.DeserializeSnapshot(row.SnapshotJson);
			var oovText = CurationContextHelper.ExtractOovText(row.SentenceJp, row.StartCharIndex, row.EndCharIndex)
				?? snap?.SelectedOovText
				?? string.Empty;

			return new CuratorOovStatDto(
				row.VideoId,
				row.VideoTitle,
				row.SegmentId,
				row.StartTimeSeconds,
				CurationContextHelper.FormatTimestamp(row.StartTimeSeconds),
				row.SentenceJp,
				row.SubtitleVn,
				row.StartCharIndex,
				row.EndCharIndex,
				oovText,
				row.ReportCount,
				row.UniqueLearnerCount,
				row.ActiveReportId,
				row.ReportStatus,
				row.FirstReportedAt,
				row.LastReportedAt,
				row.SampleFeedbacks);
		}).ToList();

		var paged = new PaginatedList<CuratorOovStatDto>(dtos, totalCount, page, pageSize);
		return Result.Success(paged);
	}
}
