using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Tracking;
using Rakushu.Application.Usecases.Learning.Tracking.GetVocabularyCoverage;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Tracking.GetVocabularyCoverage;

internal sealed class GetVocabularyCoverage : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapLearningTrackingAnalyticsEndpoints()
			.MapGet("/vocabulary-coverage", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetVocabularyCoverageQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetImmersionVocabularyCoverage")
			.WithSummary("Learner")
			.WithDescription("Case 4: Computes real immersion vocabulary coverage across JLPT levels (N5-N1), exposure depth buckets, and content domain topics.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<VocabularyCoverageOverviewDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
