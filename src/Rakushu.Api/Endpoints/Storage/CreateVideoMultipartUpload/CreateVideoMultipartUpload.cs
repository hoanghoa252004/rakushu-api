using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.CreateVideoMultipartUpload;

namespace Rakushu.Api.Endpoints.Storage.CreateVideoMultipartUpload;

internal class CreateVideoMultipartUpload : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/multipart", async (
			[FromBody] CreateVideoMultipartUploadRequestDto request,
			ISender _sender,
			CancellationToken cancellationToken
			) =>
		{
			var command = new CreateVideoMultipartUploadCommand(request.ContentType);

			var result = await _sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
			.WithName("CreateVideoMultipartUpload")
			.WithSummary("Authenticated User")
			//.RequireAuthorization()
			.Produces<CreateVideoMultipartUploadResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateVideoMultipartUploadRequestDto(
	string ContentType);