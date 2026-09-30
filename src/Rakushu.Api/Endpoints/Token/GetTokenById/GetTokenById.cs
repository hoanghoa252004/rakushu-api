using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Token.GetTokenById;

namespace Rakushu.Api.Endpoints.Token.GetTokenById;

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
