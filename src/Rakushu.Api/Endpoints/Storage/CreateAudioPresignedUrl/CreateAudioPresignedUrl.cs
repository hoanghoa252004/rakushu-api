using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.CreateAudioPresignedUrl;

namespace Rakushu.Api.Endpoints.Storage.CreateAudioPresignedUrl;

internal class CreateAudioPresignedUrl : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/audio", async (
			[FromBody] CreateAudioPresignedUrlRequestDto request,
			ISender _sender,
			CancellationToken cancellationToken
			) =>
		{
			var command = new CreateAudioPresignedUrlCommand(request.ContentType, request.FileName);

			var result = await _sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
			.WithName("CreateAudioPresignedUrl")
			.WithSummary("Authenticated User")
			.WithDescription("Create a presigned URL for uploading an audio file to storage.")
			.RequireAuthorization()
			.Produces<CreateAudioPresignedUrlResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
internal sealed record CreateAudioPresignedUrlRequestDto(string ContentType, string FileName);
