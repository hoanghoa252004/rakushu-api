using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Bunsetsu.CreateBunsetsu;

namespace Rakushu.Api.Endpoints.Bunsetsu.CreateBunsetsu;

internal sealed class CreateBunsetsu : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapBunsetsuEndpoints()
			.MapPost("/", async ([FromBody] CreateBunsetsuCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetBunsetsuById", id => new { id });
			})
			.WithName("CreateBunsetsu");
	}
}
