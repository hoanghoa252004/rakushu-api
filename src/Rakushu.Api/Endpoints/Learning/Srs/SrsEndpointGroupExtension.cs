using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.Srs;

internal static class SrsEndpointGroupExtension
{
    internal static RouteGroupBuilder MapSrsEndpoints(this IEndpointRouteBuilder app)
    {
        return app.MapGroup("/api/srs")
            .WithTags("SRS")
            .WithGroupName("srs");
    }
}
