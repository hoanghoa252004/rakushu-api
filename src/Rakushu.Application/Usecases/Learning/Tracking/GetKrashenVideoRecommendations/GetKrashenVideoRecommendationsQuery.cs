using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Tracking.GetKrashenVideoRecommendations;

public sealed record GetKrashenVideoRecommendationsQuery(
	Guid? ExcludeVideoId = null,
	int Limit = 6) : IRequest<Result<IReadOnlyList<KrashenVideoRecommendationDto>>>;
