using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.ResumeSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.ResumeSrsItem;

internal sealed class ResumeSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			.MapPatch("/items/{id:guid}/resume", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new ResumeSrsItemCommand(id);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("ResumeSrsItem")
			.WithSummary("Resume learning a suspended SRS item")
			.WithDescription("Transitions a Suspended SRS schedule back to active status, retaining previous SM-2 metrics.")
			.RequireAuthorization()
			.Produces<SrsItemDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
