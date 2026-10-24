using MediatR;
using Microsoft.AspNetCore.Mvc;
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
			.MapGet("/items", async (
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetItemsQuery(status), cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetSrsItems")
			.WithSummary("Get SRS items for the current user with optional status filter")
			.WithDescription("Returns SRS items filtered by status ('Active', 'Suspended', 'Learning', 'Review', 'Ignored'). Defaults to 'Active' (excluding Suspended and Ignored).")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<IReadOnlyList<SrsItemDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
