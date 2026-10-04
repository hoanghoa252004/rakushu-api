using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory;

internal static class ContentCategoryEndpointGroupExtension
{
	internal static RouteGroupBuilder MapContentCategoryEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/content-categories")
			.WithTags("ContentCategory")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
