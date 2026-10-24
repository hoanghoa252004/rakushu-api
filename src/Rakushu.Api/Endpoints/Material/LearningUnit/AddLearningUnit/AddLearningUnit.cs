using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Material.Transcript;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.LearningUnit.AddLearningUnit;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.AddLearningUnit;

internal sealed class AddLearningUnit : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPost("/segments/{segmentId:guid}/learning-units", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid segmentId,
				[FromBody] AddLearningUnitRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new AddLearningUnitCommand(
					videoId,
					segmentId,
					request.KnowledgeMeaningId,
					request.StartTokenIndex,
					request.EndTokenIndex);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithTags("Learning Units")
			.WithName("AddSegmentLearningUnit")
			.WithSummary("Curator")
			.WithDescription("Adds a learning unit to a segment. The knowledge meaning must exist and its knowledge unit must be active.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record AddLearningUnitRequest(
	Guid KnowledgeMeaningId,
	int StartTokenIndex,
	int EndTokenIndex);
