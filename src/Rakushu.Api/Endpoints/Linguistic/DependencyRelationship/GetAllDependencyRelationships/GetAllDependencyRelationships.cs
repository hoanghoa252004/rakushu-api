using MediatR;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetAll;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.GetAll;

internal sealed class GetAllDependencyRelationshipsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetAllDependencyRelationshipsQuery();
				var result = await sender.Send(query, cancellationToken);
				return Results.Ok(result);
			})
			.WithName("GetAllDependencyRelationships")
			.WithSummary("Admin")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError); ;
	}
}
