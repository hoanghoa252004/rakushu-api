using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Storage;

namespace Rakushu.Api.Endpoints.Storage;

internal class CreatePresignedUrl : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("api/presigned-url", async ([FromBody] CreatePresignedUrlRequestDto request, ISender _sender, CancellationToken cancellationToken) =>
		{
			var command = new CreatePresignedUrlCommand(request.ContentType);

			var result = await _sender.Send(command, cancellationToken);

			return result.MatchOk();
		})
			.WithGroupName("storage")
			.WithTags("Storage")
			.WithName("CreatePresignedUrl")
			.AllowAnonymous()
			.Produces<CreatePresignedUrlResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
internal sealed record CreatePresignedUrlRequestDto(string ContentType);