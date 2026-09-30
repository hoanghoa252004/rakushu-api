using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProfileAdmin.UpdateProfile;

namespace Rakushu.Api.Endpoints.ProfileAdmin.UpdateProfile;

internal sealed class UpdateProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProfileCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { ProfileId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateProfile");
	}
}
