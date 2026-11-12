using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.ResolveReport;

public sealed record ResolveReportCommand(
	Guid ReportId,
	string CuratorNotes) : IRequest<Result>;
