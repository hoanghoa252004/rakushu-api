using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.ResumeSrsItem;

internal sealed class ResumeSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			// 1. Endpoint
			.MapPatch("/items/{id:guid}/resume", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new ResumeSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("ResumeSrsItem")
			.WithSummary("Resume learning a suspended SRS item")
			.WithDescription("Transitions a Suspended SRS item back to Learning or Review status, keeping its previous SM-2 metrics intact.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<SrsItemDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
