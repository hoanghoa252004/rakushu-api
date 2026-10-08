using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage.CompleteVideoMultipartUpload;

namespace Rakushu.Api.Endpoints.Storage.CompleteVideoMultipartUpload;

internal class CompleteVideoMultipartUpload : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapStorageEndpoints()
			.MapPost("/multipart/complete", async (
			[FromBody] CompleteMultipartUploadRequestDto request,
			ISender sender,
			CancellationToken cancellationToken
			) =>
		{
			var parts = request.Parts
				.Select(x => new CompletedPartDto(
					x.PartNumber,
					x.ETag))
				.ToList();

			var command = new CompleteVideoMultipartUploadCommand(
				request.Key,
				request.UploadId,
				parts);

			var result = await sender.Send(
				command,
				cancellationToken);

			return result.MatchOk();
		})
			.WithName("CompleteVideoMultipartUpload")
			.WithSummary("Authenticated User")
			.RequireAuthorization()
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CompleteMultipartUploadRequestDto(
	string Key,
	string UploadId,
	IReadOnlyList<CompletedPartRequestDto> Parts);

internal sealed record CompletedPartRequestDto(
	int PartNumber,
	string ETag);