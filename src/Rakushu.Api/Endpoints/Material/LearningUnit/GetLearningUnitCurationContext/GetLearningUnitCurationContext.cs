using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitCurationContext;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.GetLearningUnitCurationContext;

internal sealed class GetLearningUnitCurationContext : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/learning-units/{learningUnitId:guid}/curation-context", async (
			[FromRoute] Guid learningUnitId,
			ISender sender,
			CancellationToken cancellationToken = default) =>
		{
			var query = new GetLearningUnitCurationContextQuery(learningUnitId);
			var result = await sender.Send(query, cancellationToken);
			return result.MatchOk();
		})
		.WithTags("Learning Units")
		.WithGroupName("material")
		.WithName("GetLearningUnitCurationContext")
		.WithSummary("Curator")
		.WithDescription("Retrieves the full video, segment, token boundaries, and subtitle curation context for a learning unit.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
		.Produces<LearningUnitCurationContextDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status403Forbidden)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
