using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Transcript.DeleteTranscript;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.DeleteTranscript;

internal sealed class DeleteTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDelete("/api/videos/{videoId:guid}/transcript", async (
			[FromRoute] Guid videoId,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new DeleteTranscriptCommand(videoId);

			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("Transcript")
		.WithGroupName("learning")
		.WithName("DeleteTranscript")
		.WithSummary("Delete transcript")
		.WithDescription("Deletes the transcript of a video. Cannot delete transcript if video is in Ready status - only update is allowed for Ready videos.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
		.Produces(StatusCodes.Status204NoContent)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status409Conflict)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
