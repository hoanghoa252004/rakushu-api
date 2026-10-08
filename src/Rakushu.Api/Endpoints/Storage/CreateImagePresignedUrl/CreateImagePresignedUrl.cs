using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.CreateImagePresignedUrl;

namespace Rakushu.Api.Endpoints.Storage.CreateImagePresignedUrl;

internal class CreateImagePresignedUrl : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/", async (
			[FromBody] CreateImagePresignedUrlRequestDto request, 
			ISender _sender, 
			CancellationToken cancellationToken
			) =>
		{
			var command = new CreateImagePresignedUrlCommand(request.ContentType);

			var result = await _sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
			.WithName("CreateImagePresignedUrl")
			.WithSummary("Authenticated User")
			.WithDescription("Create a presigned URL for uploading an image to storage.")
			.RequireAuthorization()
			.Produces<CreateImagePresignedUrlResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
internal sealed record CreateImagePresignedUrlRequestDto(string ContentType);