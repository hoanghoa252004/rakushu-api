using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Transcript.MarkTranscriptReviewed;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.MarkTranscriptReviewed;

internal sealed class MarkTranscriptReviewed : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapPatch("/is-reviewed", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new MarkTranscriptReviewedCommand(videoId), cancellationToken);

				return result.MatchOk();
			})
			.WithName("MarkTranscriptReviewed")
			.WithSummary("Curator")
			.WithDescription("Marks the transcript of a video as reviewed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
