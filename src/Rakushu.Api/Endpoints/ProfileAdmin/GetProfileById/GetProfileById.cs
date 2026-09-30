using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProfileAdmin.GetProfileById;

namespace Rakushu.Api.Endpoints.ProfileAdmin.GetProfileById;

internal sealed class GetProfileById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProfileByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProfileById");
	}
}
