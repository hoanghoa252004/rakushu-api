using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.Series;

internal static class SeriesEndpointGroupExtension
{
	internal static RouteGroupBuilder MapSeriesEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/series")
			.WithTags("Series")
			.WithGroupName("learning");
	}
}
