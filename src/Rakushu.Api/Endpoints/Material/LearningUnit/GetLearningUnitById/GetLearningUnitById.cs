using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.LearningUnit;
using Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.GetLearningUnitById;

internal sealed class GetLearningUnitById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/videos/{videoId:guid}/segments/{segmentId:guid}/learning-units/{learningUnitId:guid}", async (
			[FromRoute] Guid videoId,
			[FromRoute] Guid segmentId,
			[FromRoute] Guid learningUnitId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetLearningUnitByIdQuery(learningUnitId), cancellationToken);
				return result.MatchOk();
			})
			.WithTags("Learning Units")
			.WithGroupName("material")
			.WithName("GetLearningUnitById")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets a learning unit with its knowledge unit, knowledge collection, the used meaning, and the total meaning count of the knowledge unit.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<LearningUnitDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}