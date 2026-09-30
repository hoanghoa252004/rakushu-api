using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.DependencyRelationship.GetDependencyRelationshipById;

namespace Rakushu.Api.Endpoints.DependencyRelationship.GetDependencyRelationshipById;

internal sealed class GetDependencyRelationshipById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetDependencyRelationshipByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetDependencyRelationshipById");
	}
}
