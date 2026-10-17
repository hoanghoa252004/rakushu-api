using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.GetActiveCards;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetActiveCards;

internal sealed class GetActiveCards : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapGet("/cards/active", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetActiveCardsQuery(), cancellationToken);
                return result.MatchOk();
            })
            .WithName("GetActiveSrsCards")
            .WithSummary("Get all active SRS cards for the current user")
            .WithDescription("Returns active cards regardless of whether they are upcoming, due today, or overdue.")
            .RequireAuthorization()
            .Produces<IReadOnlyList<SrsCardDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
