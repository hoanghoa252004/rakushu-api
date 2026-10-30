using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Curator.GetCuratorReports;

internal sealed class GetCuratorReportsHandler : IRequestHandler<GetCuratorReportsQuery, Result<IReadOnlyList<CuratorReportSummaryDto>>>
{
	private readonly IReportRepository _reportRepository;

	public GetCuratorReportsHandler(IReportRepository reportRepository)
	{
		_reportRepository = reportRepository;
	}

	public async Task<Result<IReadOnlyList<CuratorReportSummaryDto>>> Handle(
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

		var dtos = items.Select(r => new CuratorReportSummaryDto(
			r.Id.Value,
			r.TargetType.ToString(),
			r.TargetId,
			r.Category,
			r.Status.ToString(),
			r.Details.Count,
			r.CreatedAt,
			r.ResolvedAt)).ToList();

		return Result.Success<IReadOnlyList<CuratorReportSummaryDto>>(dtos);
	}
}
