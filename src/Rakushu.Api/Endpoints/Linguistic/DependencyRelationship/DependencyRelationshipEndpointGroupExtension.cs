using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship;

internal static class DependencyRelationshipEndpointGroupExtension
{
	internal static RouteGroupBuilder MapDependencyRelationshipEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/dependency-relationships")
			.WithTags("DependencyRelationship")
			.WithGroupName("linguistic");
	}
}
