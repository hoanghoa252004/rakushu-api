using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.MediaAsset.GetMediaAssets;

namespace Rakushu.Api.Endpoints.Learning.MediaAsset.GetMediaAssets;

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
