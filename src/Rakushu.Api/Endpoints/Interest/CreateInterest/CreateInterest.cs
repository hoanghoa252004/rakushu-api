using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Interest.CreateInterest;

namespace Rakushu.Api.Endpoints.Interest.CreateInterest;

internal sealed class CreateInterest : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapInterestEndpoints()
			.MapPost("/", async ([FromBody] CreateInterestCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetInterestById", id => new { id });
			})
			.WithName("CreateInterest");
	}
}
