using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.GetProficiencyLevels;

internal sealed class GetProficiencyLevels : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var result = await sender.Send(new GetProficiencyLevelsQuery(), cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetProficiencyLevels")
			.WithSummary("Allow Anonymous")
			.WithDescription("Gets a list of all proficiency levels for linguistic skills.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
