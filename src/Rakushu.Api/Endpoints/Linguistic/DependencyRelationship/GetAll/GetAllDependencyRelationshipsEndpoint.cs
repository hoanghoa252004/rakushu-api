using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetAll;

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
			.WithName("GetAllDependencyRelationships");
	}
}
