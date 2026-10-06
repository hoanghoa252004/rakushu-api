using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetById;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.GetById;

internal sealed class GetDependencyRelationshipByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapGet("/{id}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetDependencyRelationshipByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);
				return result is null ? Results.NotFound() : Results.Ok(result);
			})
			.WithName("GetDependencyRelationshipById");
	}
}
