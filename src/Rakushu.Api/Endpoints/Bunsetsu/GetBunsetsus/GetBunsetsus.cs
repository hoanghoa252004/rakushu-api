using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Bunsetsu.GetBunsetsus;

namespace Rakushu.Api.Endpoints.Bunsetsu.GetBunsetsus;

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
