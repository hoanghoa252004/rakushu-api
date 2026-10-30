using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.GetMyIssueHistory;

namespace Rakushu.Api.Endpoints.Curation.GetMyIssueHistory;

internal sealed class GetMyIssueHistory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerCurationEndpoints()
			.MapGet("/my-history", async (
				[FromQuery] int page,
				[FromQuery] int pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetMyIssueHistoryQuery(
					page <= 0 ? 1 : page,
					pageSize <= 0 ? 20 : pageSize);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyIssueHistory")
			.WithSummary("Get current learner's submitted issue reports")
			.WithDescription("Retrieves all issue reports submitted by the logged-in learner and their current review/escalation status.")
			.RequireAuthorization()
			.Produces<IReadOnlyList<LearnerIssueHistoryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
