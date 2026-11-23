using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Statistics;
using Rakushu.Application.Usecases.Learning.Srs.Statistics.GetSrsCoverageStatistics;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.Statistics.GetSrsCoverageStatistics;

internal sealed class GetSrsCoverageStatistics : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsStatisticsEndpoints()
			.MapGet("/coverage", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetSrsCoverageStatisticsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSrsCoverageStatistics")
			.WithSummary("Learner")
			.WithDescription("Returns the learner's vocabulary & grammar coverage matrix across JLPT levels (N5-N1) and knowledge types (Word, Grammar, Collocation, Terminology, ProperNoun), plus aggregated summaries.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SrsCoverageStatisticsDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
