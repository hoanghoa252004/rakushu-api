using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Bunsetsu.GetBunsetsuById;

namespace Rakushu.Api.Endpoints.Learning.Bunsetsu.GetBunsetsuById;

internal sealed class GetBunsetsuById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapBunsetsuEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetBunsetsuByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetBunsetsuById");
	}
}
