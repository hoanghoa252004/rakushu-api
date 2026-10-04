using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Token.DeleteToken;

namespace Rakushu.Api.Endpoints.Learning.Token.DeleteToken;

internal sealed class DeleteToken : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTokenEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteTokenCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteToken");
	}
}
