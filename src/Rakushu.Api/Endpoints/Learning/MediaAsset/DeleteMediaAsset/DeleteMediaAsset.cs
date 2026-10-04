using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.MediaAsset.DeleteMediaAsset;

namespace Rakushu.Api.Endpoints.Learning.MediaAsset.DeleteMediaAsset;

internal sealed class DeleteMediaAsset : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapMediaAssetEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteMediaAssetCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteMediaAsset");
	}
}
