using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.GetVideoById;

namespace Rakushu.Api.Endpoints.Learning.Video.GetVideoById;

internal sealed class GetVideoById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetVideoByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetVideoById");
	}
}
