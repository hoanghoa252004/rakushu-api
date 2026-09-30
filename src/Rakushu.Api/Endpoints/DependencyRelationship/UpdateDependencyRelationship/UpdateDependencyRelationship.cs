using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.DependencyRelationship.UpdateDependencyRelationship;

namespace Rakushu.Api.Endpoints.DependencyRelationship.UpdateDependencyRelationship;

internal sealed class UpdateDependencyRelationship : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateDependencyRelationshipCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { DependencyRelationshipId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateDependencyRelationship");
	}
}
