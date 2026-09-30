using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProfileAdmin.CreateProfile;

namespace Rakushu.Api.Endpoints.ProfileAdmin.CreateProfile;

internal sealed class CreateProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapPost("/", async ([FromBody] CreateProfileCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetProfileById", id => new { id });
			})
			.WithName("CreateProfile");
	}
}
