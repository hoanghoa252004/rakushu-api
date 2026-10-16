using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Knowledge.ProficiencyLevel;

internal static class ProficiencyLevelEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProficiencyLevelEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/proficiency-levels")
			.WithTags("ProficiencyLevel")
			.WithGroupName("knowledge");
	}
}
