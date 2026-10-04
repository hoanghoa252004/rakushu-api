using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.DependencyRelationship;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.CreateDependencyRelationship;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.CreateDependencyRelationship;

internal sealed class CreateDependencyRelationship : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapPost("/", async ([FromBody] CreateDependencyRelationshipCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetDependencyRelationshipById", id => new { id });
			})
			.WithName("CreateDependencyRelationship");
	}
}
