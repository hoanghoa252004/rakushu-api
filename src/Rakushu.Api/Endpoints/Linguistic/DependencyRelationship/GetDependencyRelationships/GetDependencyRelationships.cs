using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.DependencyRelationship.GetDependencyRelationships;

namespace Rakushu.Api.Endpoints.Linguistic.DependencyRelationship.GetDependencyRelationships;

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
