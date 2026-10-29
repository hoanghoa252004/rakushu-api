using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.Srs;

internal static class SrsEndpointGroupExtension
{
	internal static RouteGroupBuilder MapSrsItemEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs")
			.WithTags("SRS Items")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsQuizEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs")
			.WithTags("SRS Quizzes")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsFlashcardEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs")
			.WithTags("SRS Flashcards")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs")
			.WithTags("SRS")
			.WithGroupName("srs");
	}
}
