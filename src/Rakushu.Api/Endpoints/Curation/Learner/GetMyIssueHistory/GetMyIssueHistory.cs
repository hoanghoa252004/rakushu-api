using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curation;
using Rakushu.Application.Usecases.Curation.Learner.GetMyIssueHistory;

namespace Rakushu.Api.Endpoints.Curation.Learner.GetMyIssueHistory;

internal sealed class GetMyIssueHistory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerCurationEndpoints()
			.MapGet("/my-history", async (
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetMyIssueHistoryQuery(
					page is null or <= 0 ? 1 : page.Value,
					pageSize is null or <= 0 ? 20 : pageSize.Value);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyIssueHistory")
			.WithSummary("Learner")
			.WithDescription("Retrieves a paginated list of issue reports submitted by the currently authenticated learner along with their review status.")
			.RequireAuthorization()
			.Produces<PaginatedList<LearnerIssueHistoryDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
