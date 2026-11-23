using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetVocabularyCoverage;

public sealed record GetVocabularyCoverageQuery : IRequest<Result<VocabularyCoverageOverviewDto>>;
