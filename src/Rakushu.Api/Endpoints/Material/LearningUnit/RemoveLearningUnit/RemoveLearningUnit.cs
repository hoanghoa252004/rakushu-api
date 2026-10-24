using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Material.Transcript;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.LearningUnit.RemoveLearningUnit;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.RemoveLearningUnit;

internal sealed class RemoveLearningUnit : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapDelete("/segments/{segmentId:guid}/learning-units/{learningUnitId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromRoute] Guid learningUnitId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new RemoveLearningUnitCommand(videoId, segmentId, learningUnitId), cancellationToken);
				return result.MatchOk();
			})
			.WithTags("Learning Units")
			.WithName("RemoveSegmentLearningUnit")
			.WithSummary("Curator")
			.WithDescription("Removes a learning unit from a segment.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
