using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.Learner.GetMyIssueSummary;

namespace Rakushu.Api.Endpoints.Curation.Learner.GetMyIssueSummary;

internal sealed class GetMyIssueSummary : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerCurationEndpoints()
			.MapGet("/my-summary", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetMyIssueSummaryQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyIssueSummary")
			.WithSummary("Learner")
			.WithDescription("Retrieves aggregate report counts by status for the currently authenticated learner.")
			.RequireAuthorization()
			.Produces<LearnerIssueSummaryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
