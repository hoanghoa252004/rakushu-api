using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.JapaneseConjugationForm;

internal static class JapaneseConjugationFormEndpointGroupExtension
{
	internal static RouteGroupBuilder MapJapaneseConjugationFormEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/admin/japanese-conjugation-forms")
			.WithTags("JapaneseConjugationForm")
			.WithGroupName("admin")
			.AllowAnonymous();
	}
}
