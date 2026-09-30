using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProfileAdmin.GetProfiles;

namespace Rakushu.Api.Endpoints.ProfileAdmin.GetProfiles;

internal sealed class GetProfiles : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProfilesQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProfiles");
	}
}
