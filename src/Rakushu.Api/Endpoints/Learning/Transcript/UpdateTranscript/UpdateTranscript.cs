using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript;
using Rakushu.Application.Usecases.Learning.Transcript.UpdateTranscript;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.Transcript.UpdateTranscript;

internal sealed class UpdateTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPut("/api/videos/{videoId:guid}/transcript", async (
			[FromRoute] Guid videoId,
			[FromBody] UpdateTranscriptRequest request,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new UpdateTranscriptCommand(
				videoId,
				request.FullText,
				request.StorageKey);

			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("Transcript")
		.WithGroupName("learning")
		.WithName("UpdateTranscript")
		.WithSummary("Update transcript")
		.WithDescription("Updates the transcript of a video. Only FullText and StorageKey can be updated. This works for all video statuses.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
		.Produces(StatusCodes.Status200OK)
		.ProducesValidationProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateTranscriptRequest(
	string FullText,
	string StorageKey);
