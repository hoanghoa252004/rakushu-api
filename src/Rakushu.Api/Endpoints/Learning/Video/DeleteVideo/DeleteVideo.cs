using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Video.DeleteVideo;

namespace Rakushu.Api.Endpoints.Learning.Video.DeleteVideo;

internal sealed class DeleteVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapVideoEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteVideoCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteVideo");
	}
}
