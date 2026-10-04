using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Token;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Token.UpdateToken;

namespace Rakushu.Api.Endpoints.Learning.Token.UpdateToken;

internal sealed class UpdateToken : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTokenEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTokenCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { TokenId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateToken");
	}
}
