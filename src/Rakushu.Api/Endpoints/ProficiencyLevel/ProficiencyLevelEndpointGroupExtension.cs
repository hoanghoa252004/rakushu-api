using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.ProficiencyLevel;

internal static class ProficiencyLevelEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProficiencyLevelEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/proficiency-levels")
			.WithTags("ProficiencyLevel")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
