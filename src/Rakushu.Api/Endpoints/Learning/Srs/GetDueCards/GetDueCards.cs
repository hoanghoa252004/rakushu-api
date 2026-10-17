using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.GetDueCards;

namespace Rakushu.Api.Endpoints.Learning.Srs.GetDueCards;

internal sealed class GetDueCards : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapSrsEndpoints()
            .MapGet("/cards/due", async (ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetDueCardsQuery(), cancellationToken);
                return result.MatchOk();
            })
            .WithName("GetDueSrsCards")
            .WithSummary("Get SRS cards due for review")
            .WithDescription("Returns active cards whose scheduled due time is now or earlier for the current user.")
            .RequireAuthorization()
            .Produces<IReadOnlyList<SrsCardDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}