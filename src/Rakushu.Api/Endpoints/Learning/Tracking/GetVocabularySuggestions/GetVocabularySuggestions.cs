using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.GetVocabularySuggestions;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.GetVocabularySuggestions;

internal sealed class GetVocabularySuggestions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningTrackingAnalyticsEndpoints()
			.MapGet("/vocabulary-suggestions", async (
				[FromQuery] int? limit,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetVocabularySuggestionsQuery(limit ?? 12);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetEncounteredVocabularySuggestions")
			.WithSummary("Learner")
			.WithDescription("Case 1: Recommends high-frequency or cross-video encountered vocabulary not yet saved to SRS flashcards, prioritized by the learner's target JLPT level and content interests, with video context provenance.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<IReadOnlyList<EncounteredVocabularySuggestionDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
