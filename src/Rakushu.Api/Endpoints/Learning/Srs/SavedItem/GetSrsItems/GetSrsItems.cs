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
				[FromQuery] string? filter,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				[FromQuery] string? sortBy,
				[FromQuery] string? sortOrder,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsItemsQuery(page ?? 1, pageSize ?? 50, filter, sortBy, sortOrder);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsItems")
			.WithSummary("Learner")
			.WithDescription("Returns a lightweight list of saved vocabulary items with basic word details, SM-2 metrics, and due dates. Filter by filter ('all', 'due', 'leech', 'mastered'). Supports sorting via sortBy ('saved', 'due', 'leech', 'alphabetical') and sortOrder ('asc', 'desc'). For deep video context, call GET /api/srs/saved-items/{id}.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<SavedItemSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
