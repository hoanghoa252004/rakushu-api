using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Curator.MarkReportInReview;

public sealed record MarkReportInReviewCommand(Guid ReportId) : IRequest<Result>;
