using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.GetCards;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetCards;

internal sealed class GetCards : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			.MapGet("/cards", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetCardsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsCards")
			.WithSummary("Get all SRS cards for the current user sorted by review schedule")
			.WithDescription("Returns enrolled active cards sorted by DueAt ascending (cards due soonest or most overdue first).")
			.RequireAuthorization()
			.Produces<IReadOnlyList<SrsCardDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized);
	}
}
