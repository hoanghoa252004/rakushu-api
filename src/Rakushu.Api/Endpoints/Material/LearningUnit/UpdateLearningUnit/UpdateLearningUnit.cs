using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Material.Transcript;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.LearningUnit.UpdateLearningUnit;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.UpdateLearningUnit;

internal sealed class UpdateLearningUnit : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPut("/segments/{segmentId:guid}/learning-units/{learningUnitId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromRoute] Guid learningUnitId,
				[FromBody] UpdateLearningUnitRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new UpdateLearningUnitCommand(
					videoId,
					segmentId,
					learningUnitId,
					request.KnowledgeMeaningId,
					request.StartTokenIndex,
					request.EndTokenIndex);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithTags("Learning Units")
			.WithName("UpdateSegmentLearningUnit")
			.WithSummary("Curator")
			.WithDescription("Updates a learning unit of a segment (full update). The knowledge meaning must exist and its knowledge unit must be active.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateLearningUnitRequest(
	Guid KnowledgeMeaningId,
	int StartTokenIndex,
	int EndTokenIndex);
