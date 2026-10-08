using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.CreateVideoPresignedPartUrl;

namespace Rakushu.Api.Endpoints.Storage.CreateVideoPresignedPartUrl;

internal class CreateVideoPresignedPartUrl : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/multipart/parts", async (
			[FromBody] CreateVideoPresignedPartUrlRequestDto request,
			ISender sender,
			CancellationToken cancellationToken
			) =>
		{
			var command = new CreateVideoPresignedPartUrlCommand(
				request.Key,
				request.UploadId,
				request.FileSize);

			var result = await sender.Send(
				command,
				cancellationToken);

			return result.MatchOk();
		})
			.WithName("CreateVideoPresignedPartUrls")
			.WithSummary("Authenticated User")
			.RequireAuthorization()
			.Produces<CreateVideoPresignedPartUrlResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateVideoPresignedPartUrlRequestDto(
	string Key,
	string UploadId,
	long FileSize);