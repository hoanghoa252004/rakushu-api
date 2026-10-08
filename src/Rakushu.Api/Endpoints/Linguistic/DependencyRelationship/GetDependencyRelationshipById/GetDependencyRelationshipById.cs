using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.GetDependencyRelationshipById;

internal sealed class GetDependencyRelationshipById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapGet("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetDependencyRelationshipByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);
				return result is null ? Results.NotFound() : Results.Ok(result);
			})
			.WithName("GetDependencyRelationshipById")
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
