using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Interest.GetInterests;

namespace Rakushu.Api.Endpoints.Interest.GetInterests;

internal sealed class GetInterests : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapInterestEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetInterestsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetInterests");
	}
}
