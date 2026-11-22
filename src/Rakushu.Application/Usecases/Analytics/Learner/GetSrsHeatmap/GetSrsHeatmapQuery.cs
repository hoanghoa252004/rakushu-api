using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Learner.GetSrsHeatmap;

public sealed record GetSrsHeatmapQuery(
	int? Year = null,
	string? TimeZone = null) : IRequest<Result<SrsHeatmapResponseDto>>;
