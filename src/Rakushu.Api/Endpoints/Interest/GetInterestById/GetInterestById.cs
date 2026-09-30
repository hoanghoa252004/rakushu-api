using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Interest.GetInterestById;

namespace Rakushu.Api.Endpoints.Interest.GetInterestById;

internal sealed class GetInterestById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapInterestEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetInterestByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetInterestById");
	}
}
