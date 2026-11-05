using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.GetCuratorReports;

public sealed record GetCuratorReportsQuery(
	string? Status = null,
	string? TargetType = null,
	string? Category = null,
	int Page = 1,
	int PageSize = 20) : IRequest<Result<IReadOnlyList<CuratorReportSummaryDto>>>;
