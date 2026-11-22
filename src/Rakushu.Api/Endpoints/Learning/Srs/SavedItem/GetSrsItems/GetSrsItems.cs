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
				[FromQuery] string? level,
				[FromQuery] string? knowledgeType,
				[FromQuery] string? status,
				[FromQuery] string? search,
				[FromQuery] string? dueDate,
				[FromQuery] string? timeZone,
				[FromQuery] bool? isActive,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsItemsQuery(
					page ?? 1,
					pageSize ?? 50,
					filter,
					sortBy,
					sortOrder,
					level,
					knowledgeType,
					status,
					search,
					dueDate,
					timeZone,
					isActive);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsItems")
			.WithSummary("Learner")
			.WithDescription("Returns saved vocabulary items with word details, SM-2 metrics, JLPT level, knowledge type, and maturity status. Supports filtering by filter, level, knowledgeType, status, search, dueDate (yyyy-MM-dd), timeZone, and isActive, plus sorting via sortBy ('saved', 'due', 'leech', 'alphabetical', 'interval') and sortOrder.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<SavedItemSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
