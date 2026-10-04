using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Token.GetTokenById;

namespace Rakushu.Api.Endpoints.Learning.Token.GetTokenById;

internal sealed class GetTokenById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTokenEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTokenByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTokenById");
	}
}
