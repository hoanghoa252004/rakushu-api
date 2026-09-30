using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.DependencyRelationship.GetDependencyRelationships;

namespace Rakushu.Api.Endpoints.DependencyRelationship.GetDependencyRelationships;

internal sealed class GetDependencyRelationships : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapDependencyRelationshipEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetDependencyRelationshipsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetDependencyRelationships");
	}
}
