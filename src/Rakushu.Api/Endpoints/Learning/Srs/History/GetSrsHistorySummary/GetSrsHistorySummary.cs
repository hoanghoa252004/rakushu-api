using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.History;
using Rakushu.Application.Usecases.Learning.Srs.History.GetSrsHistorySummary;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.History.GetSrsHistorySummary;

internal sealed class GetSrsHistorySummary : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsHistoryEndpoints()
			.MapGet("/summary", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsHistorySummaryQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsHistorySummary")
			.WithSummary("Learner")
			.WithDescription("Retrieves aggregate flashcard rating distribution (easy, hard, forgot) and quiz answer outcome distribution (correct, incorrect, unanswered) for the currently authenticated learner.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SrsHistorySummaryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
