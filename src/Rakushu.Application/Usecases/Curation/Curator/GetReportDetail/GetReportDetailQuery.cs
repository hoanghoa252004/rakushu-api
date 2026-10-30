using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.GetReportDetail;

public sealed record GetReportDetailQuery(Guid ReportId) : IRequest<Result<CuratorReportDetailDto>>;
