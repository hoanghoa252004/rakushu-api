using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.GetSrsItems;

internal sealed class GetSrsItems : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsSavedItemEndpoints()
			.MapGet("", async (
				[FromQuery] bool? isActive,
				[FromQuery] bool? dueOnly,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsItemsQuery(isActive, dueOnly, page ?? 1, pageSize ?? 50);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsItems")
			.WithSummary("Learner")
			.WithDescription("Returns a lightweight list of saved vocabulary items with basic word details and due dates. Filter by isActive (true/false) and dueOnly (true/false). For deep video context, call GET /api/srs/saved-items/{id}.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<SavedItemSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
