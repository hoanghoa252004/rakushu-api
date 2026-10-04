using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework;

internal static class ProficiencyFrameworkEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProficiencyFrameworkEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/proficiency-frameworks")
			.WithTags("ProficiencyFramework")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
