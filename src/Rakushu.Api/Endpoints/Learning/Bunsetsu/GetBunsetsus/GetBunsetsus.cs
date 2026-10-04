using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Bunsetsu.GetBunsetsus;

namespace Rakushu.Api.Endpoints.Learning.Bunsetsu.GetBunsetsus;

internal sealed class GetBunsetsus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapBunsetsuEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetBunsetsusQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetBunsetsus");
	}
}
