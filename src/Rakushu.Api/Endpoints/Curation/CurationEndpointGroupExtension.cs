using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Curation;

internal static class CurationEndpointGroupExtension
{
	internal static RouteGroupBuilder MapLearnerCurationEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/curation/issues")
			.WithTags("Curation - Learner")
			.WithGroupName("curation");
	}

	internal static RouteGroupBuilder MapCuratorCurationEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/curation/reports")
			.WithTags("Curation - Curator")
			.WithGroupName("curation");
	}
}
