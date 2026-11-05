using MediatR;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Application.Usecases.Curation.Curator.GetReportDetail;

internal sealed class GetReportDetailHandler : IRequestHandler<GetReportDetailQuery, Result<CuratorReportDetailDto>>
{
	private readonly IReportRepository _reportRepository;

	public GetReportDetailHandler(IReportRepository reportRepository)
	{
		_reportRepository = reportRepository;
	}

	public async Task<Result<CuratorReportDetailDto>> Handle(
		GetReportDetailQuery request,
		CancellationToken cancellationToken)
	{
		var reportId = ReportId.From(request.ReportId);
		var report = await _reportRepository.GetByIdWithDetailsAsync(reportId, cancellationToken);
		if (report is null)
			return Result.Failure<CuratorReportDetailDto>(ReportErrors.NotFound);

		var feedbacks = report.Details.Select(d => new IssueFeedbackDto(
			d.UserId.Value,
			d.User?.FullName,
			d.LearnerFeedback,
			d.CreatedAt)).ToList();

		var dto = new CuratorReportDetailDto(
			report.Id.Value,
			report.TargetType.ToString(),
			report.TargetId,
			report.Category,
			report.Status.ToString(),
			report.CuratorNotes,
			report.CreatedAt,
			report.ResolvedAt,
			feedbacks);

		return Result.Success(dto);
	}
}
