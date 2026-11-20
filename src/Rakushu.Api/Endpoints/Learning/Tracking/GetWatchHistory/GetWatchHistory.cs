using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.GetWatchHistory;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.GetWatchHistory;

internal sealed class GetWatchHistory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningVideoTrackingEndpoints()
			.MapGet("/watch-history", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetWatchHistoryQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetWatchHistory")
			.WithSummary("Learner")
			.WithDescription("Retrieves the learner's full watched video history with merged watched intervals, completion coverage, and encountered learning units per video.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<WatchedVideoHistoryItemDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
