using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.MediaAsset.GetMediaAssets;

namespace Rakushu.Api.Endpoints.MediaAsset.GetMediaAssets;

internal sealed class GetMediaAssets : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapMediaAssetEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetMediaAssetsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMediaAssets");
	}
}
