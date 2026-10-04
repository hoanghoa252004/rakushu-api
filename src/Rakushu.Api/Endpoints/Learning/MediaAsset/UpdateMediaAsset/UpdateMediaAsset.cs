using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.MediaAsset;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.MediaAsset.UpdateMediaAsset;

namespace Rakushu.Api.Endpoints.Learning.MediaAsset.UpdateMediaAsset;

internal sealed class UpdateMediaAsset : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapMediaAssetEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateMediaAssetCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { MediaAssetId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateMediaAsset");
	}
}
