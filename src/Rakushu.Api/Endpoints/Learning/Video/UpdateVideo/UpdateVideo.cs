using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Video;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.UpdateVideo;

namespace Rakushu.Api.Endpoints.Learning.Video.UpdateVideo;

internal sealed class UpdateVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateVideoCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { VideoId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateVideo");
	}
}
