using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Interest.DeleteInterest;

namespace Rakushu.Api.Endpoints.Interest.DeleteInterest;

internal sealed class DeleteInterest : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapInterestEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteInterestCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteInterest");
	}
}
