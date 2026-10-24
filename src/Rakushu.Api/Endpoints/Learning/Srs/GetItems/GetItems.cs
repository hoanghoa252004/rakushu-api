using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.GetItems;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetItems;

internal sealed class GetItems : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapGet("/items", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetItemsQuery(), cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetSrsItems")
			.WithSummary("Get all SRS items for the current user sorted by review schedule")
			.WithDescription("Returns enrolled active items sorted by DueAt ascending (items due soonest or most overdue first).")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<IReadOnlyList<SrsItemDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
