using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

internal sealed class GetProficiencyLevelById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			// 1. Endpoint
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var result = await sender.Send(new GetProficiencyLevelByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetProficiencyLevelById")
			.WithSummary("Allow Anonymous")
			.WithDescription("Gets a proficiency level by its unique identifier.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces<ProficiencyLevelDto>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
