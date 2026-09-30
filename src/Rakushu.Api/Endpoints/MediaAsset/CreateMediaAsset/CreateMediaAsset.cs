using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.MediaAsset.CreateMediaAsset;

namespace Rakushu.Api.Endpoints.MediaAsset.CreateMediaAsset;

internal sealed class CreateMediaAsset : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapMediaAssetEndpoints()
			.MapPost("/", async ([FromBody] CreateMediaAssetCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetMediaAssetById", id => new { id });
			})
			.WithName("CreateMediaAsset");
	}
}
