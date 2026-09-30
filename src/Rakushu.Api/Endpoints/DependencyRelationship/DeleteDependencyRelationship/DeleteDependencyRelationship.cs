using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.DependencyRelationship.DeleteDependencyRelationship;

namespace Rakushu.Api.Endpoints.DependencyRelationship.DeleteDependencyRelationship;

internal sealed class DeleteDependencyRelationship : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteDependencyRelationshipCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteDependencyRelationship");
	}
}
