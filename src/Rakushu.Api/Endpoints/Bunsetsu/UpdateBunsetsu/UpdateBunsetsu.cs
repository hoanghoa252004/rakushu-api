using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Bunsetsu.UpdateBunsetsu;

namespace Rakushu.Api.Endpoints.Bunsetsu.UpdateBunsetsu;

internal sealed class UpdateBunsetsu : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapBunsetsuEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateBunsetsuCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { BunsetsuId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateBunsetsu");
	}
}
