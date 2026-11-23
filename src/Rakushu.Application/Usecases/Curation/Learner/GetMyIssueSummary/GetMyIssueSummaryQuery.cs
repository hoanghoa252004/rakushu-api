using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Learner.GetMyIssueSummary;

public sealed record GetMyIssueSummaryQuery : IRequest<Result<LearnerIssueSummaryDto>>;
