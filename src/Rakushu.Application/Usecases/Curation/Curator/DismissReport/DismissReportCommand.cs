using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.DismissReport;

public sealed record DismissReportCommand(
	Guid ReportId,
	string Reason) : IRequest<Result>;
