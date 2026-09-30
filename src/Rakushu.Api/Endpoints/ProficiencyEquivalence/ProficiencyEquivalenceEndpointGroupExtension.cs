using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.ProficiencyEquivalence;

internal static class ProficiencyEquivalenceEndpointGroupExtension
{
	internal static RouteGroupBuilder MapProficiencyEquivalenceEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/proficiency-equivalences")
			.WithTags("ProficiencyEquivalence")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
