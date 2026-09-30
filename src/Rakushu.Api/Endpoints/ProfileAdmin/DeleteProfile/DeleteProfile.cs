using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProfileAdmin.DeleteProfile;

namespace Rakushu.Api.Endpoints.ProfileAdmin.DeleteProfile;

internal sealed class DeleteProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteProfileCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteProfile");
	}
}
