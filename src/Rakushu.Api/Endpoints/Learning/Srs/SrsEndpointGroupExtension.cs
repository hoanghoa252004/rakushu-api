using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Rakushu.Api.Endpoints.Learning.Srs;

internal static class SrsEndpointGroupExtension
{
	internal static RouteGroupBuilder MapSrsSavedItemEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs/saved-items")
			.WithTags("Saved Item")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsItemEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapSrsSavedItemEndpoints();
	}

	internal static RouteGroupBuilder MapSrsScheduleEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs/schedules")
			.WithTags("Schedule")
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
		return app.MapGroup("/api/srs/flashcard-sessions")
			.WithTags("Flashcard")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsQuestionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs/questions")
			.WithTags("Question")
			.WithGroupName("srs");
	}

	internal static RouteGroupBuilder MapSrsBankQuestionEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapSrsQuestionEndpoints();
	}

	internal static RouteGroupBuilder MapSrsEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/srs")
			.WithTags("SRS")
			.WithGroupName("srs");
	}
}
