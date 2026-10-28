using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.SaveSrsItem;

namespace Rakushu.Api.Endpoints.Learning.Srs.SaveSrsItem;

internal sealed class SaveSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsEndpoints()
			.MapPost("/items", async (
				[FromBody] SaveSrsItemRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SaveSrsItemCommand(dto.MeaningId);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetSrsItems", item => new { id = item.Id });
			})
			.WithName("SaveSrsItem")
			.WithSummary("Save a vocabulary meaning into personal SRS notebook")
			.WithDescription("Creates a saved item snapshot and initializes its SM-2 review schedule with New status and due today.")
			.RequireAuthorization()
			.Produces<SrsItemDto>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SaveSrsItemRequestDto(Guid MeaningId);
