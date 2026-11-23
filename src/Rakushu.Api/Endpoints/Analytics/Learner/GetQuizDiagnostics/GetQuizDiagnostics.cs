using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Learner;
using Rakushu.Application.Usecases.Analytics.Learner.GetQuizDiagnostics;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Learner.GetQuizDiagnostics;

internal sealed class GetQuizDiagnostics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearnerAnalyticsEndpoints()
			.MapGet("/quiz-diagnostics", async (
				[FromQuery] int? days,
				[FromQuery] string? questionTypeCode,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetQuizDiagnosticsQuery(days, questionTypeCode), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetQuizDiagnostics")
			.WithSummary("Learner")
			.WithDescription("Returns accuracy, speed vs expected duration, weakest-type identification, and per-word evidence across all 7 question types (WORD_TO_MEANING, KANJI_TO_READING, READING_TO_KANJI, TYPE_READING, TEXT_CLOZE, VIDEO_CLOZE, AUDIO_DICTATION).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<QuestionTypeDetailResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
