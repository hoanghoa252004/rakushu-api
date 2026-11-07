using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.SaveSrsItem;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.SaveSrsItem;

internal sealed class SaveSrsItem : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsSavedItemEndpoints()
			.MapPost("", async (
				[FromBody] SaveSrsItemRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new SaveSrsItemCommand(dto.MeaningId);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetSavedItemById", id => new { id });
			})
			.WithName("SaveSrsItem")
			.WithSummary("Learner")
			.WithDescription("Accepts a knowledge meaning ID, creates a saved item and initializes its SM-2 review schedule.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record SaveSrsItemRequestDto(Guid MeaningId);
