using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.MediaAsset.GetMediaAssetById;

namespace Rakushu.Api.Endpoints.Learning.MediaAsset.GetMediaAssetById;

internal sealed class GetMediaAssetById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapMediaAssetEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetMediaAssetByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMediaAssetById");
	}
}
