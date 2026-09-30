using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Interest.UpdateInterest;

namespace Rakushu.Api.Endpoints.Interest.UpdateInterest;

internal sealed class UpdateInterest : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapInterestEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateInterestCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { InterestId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateInterest");
	}
}
