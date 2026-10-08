using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.AbortVideoMultipartUpload;

namespace Rakushu.Api.Endpoints.Storage.AbortVideoMultipartUpload;

internal class AbortVideoMultipartUpload : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/multipart/abort", async (
			[FromBody] AbortVideoMultipartUploadRequestDto request,
			ISender sender,
			CancellationToken cancellationToken
			) =>
		{
			var command = new AbortVideoMultipartUploadCommand(
				request.Key,
				request.UploadId);

			var result = await sender.Send(
				command,
				cancellationToken);

			return result.MatchOk();
		})
			.WithName("AbortVideoMultipartUpload")
			.WithSummary("Authenticated User")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record AbortVideoMultipartUploadRequestDto(
	string Key,
	string UploadId);