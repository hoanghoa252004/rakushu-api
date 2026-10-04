using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Bunsetsu.DeleteBunsetsu;

namespace Rakushu.Api.Endpoints.Learning.Bunsetsu.DeleteBunsetsu;

internal sealed class DeleteBunsetsu : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapBunsetsuEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteBunsetsuCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteBunsetsu");
	}
}
