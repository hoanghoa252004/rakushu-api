using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.GetKrashenVideoRecommendations;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.GetKrashenVideoRecommendations;

internal sealed class GetKrashenVideoRecommendations : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningTrackingAnalyticsEndpoints()
			.MapGet("/video-recommendations", async (
				[FromQuery] Guid? excludeVideoId,
				[FromQuery] int? limit,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetKrashenVideoRecommendationsQuery(
					excludeVideoId,
					limit ?? 6);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetKrashenVideoRecommendations")
			.WithSummary("Learner")
			.WithDescription("Case 3: Recommends next immersion videos using Stephen Krashen's i+1 Comprehensible Input formula (targeting 80%-85% known vocabulary and 15%-20% new vocabulary).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<KrashenVideoRecommendationDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
