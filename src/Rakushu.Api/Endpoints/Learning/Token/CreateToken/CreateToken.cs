using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.Token;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Token.CreateToken;

namespace Rakushu.Api.Endpoints.Learning.Token.CreateToken;

internal sealed class CreateToken : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTokenEndpoints()
			.MapPost("/", async ([FromBody] CreateTokenCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetTokenById", id => new { id });
			})
			.WithName("CreateToken");
	}
}
