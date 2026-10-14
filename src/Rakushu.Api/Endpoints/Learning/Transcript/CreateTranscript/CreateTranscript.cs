using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Transcript.CreateTranscript;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Transcript.CreateTranscript;

internal sealed class CreateTranscript : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/videos/{videoId:guid}/transcript", async (
			Guid videoId,
			[FromBody] CreateTranscriptRequest request,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var command = new CreateTranscriptCommand(
				videoId,
				request.FullText,
				request.StorageKey);

			var result = await sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
		.WithTags("Transcript")
		.WithGroupName("learning")
		.WithName("CreateTranscript")
		.WithSummary("Add transcript to video")
		.WithDescription("Creates a new transcript for the specified video with Official source. Only one transcript per video is allowed. StorageKey is required.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
		.Produces(StatusCodes.Status201Created)
		.ProducesValidationProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateTranscriptRequest(
	string FullText,
	string StorageKey);
