using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.GetSrsItems;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetSrsItems;

internal sealed class GetSrsItems : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			.MapGet("/items", async (
				[FromQuery] string? status,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSrsItemsQuery(status, page ?? 1, pageSize ?? 50);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsItems")
			.WithSummary("Get saved vocabulary items for the current user")
			.WithDescription("Returns saved vocabulary items with their SM-2 scheduling metrics. Filter by status: 'All', 'Due_today' (or 'Due'), 'Overdue', 'Learning', 'Review', 'Suspended'.")
			.RequireAuthorization()
			.Produces<IReadOnlyList<SrsItemDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
