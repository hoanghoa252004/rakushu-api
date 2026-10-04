using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Video;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.CreateVideo;

namespace Rakushu.Api.Endpoints.Learning.Video.CreateVideo;

internal sealed class CreateVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapPost("/", async ([FromBody] CreateVideoCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetVideoById", id => new { id });
			})
			.WithName("CreateVideo");
	}
}
