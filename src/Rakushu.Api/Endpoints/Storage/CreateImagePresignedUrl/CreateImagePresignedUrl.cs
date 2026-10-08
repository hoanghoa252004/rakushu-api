using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage;
using Rakushu.Application.Usecases.Storage.CreateImagePresignedUrl;

namespace Rakushu.Api.Endpoints.Storage.CreateImagePresignedUrl;

internal class CreateImagePresignedUrl : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("/api/presigned-url/images", async (
			[FromBody] CreateImagePresignedUrlRequestDto request, 
			ISender _sender, CancellationToken cancellationToken
			) =>
		{
			var command = new CreateImagePresignedUrlCommand(request.ContentType);

			var result = await _sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
			.WithTags("Storage")
			.WithGroupName("storage")
			.WithName("CreateImagePresignedUrl")
			.WithSummary("Authenticated User")
			.RequireAuthorization()
			.Produces<PresignedUrlResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
internal sealed record CreateImagePresignedUrlRequestDto(string ContentType);