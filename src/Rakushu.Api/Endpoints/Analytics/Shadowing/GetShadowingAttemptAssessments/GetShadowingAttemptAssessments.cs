using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Shadowing.GetShadowingAttemptAssessments;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Shadowing.GetShadowingAttemptAssessments;

internal sealed class GetShadowingAttemptAssessments : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingAnalyticsEndpoints()
			.MapGet("/sessions/{sessionId:guid}/attempts/{attemptId:guid}", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid attemptId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetShadowingAttemptAssessmentsQuery(sessionId, attemptId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingAttemptAnalyticsDetail")
			.WithSummary("Learner (owner), Curator & Admin")
			.WithDescription("Returns full Level-3 attempt analytics with AI/Curator assessments, per-line accuracy & pitch scores, Azure speech word breakdown, presigned user & native reference audio URLs, and feedback.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<ShadowingAttemptAssessmentsResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
