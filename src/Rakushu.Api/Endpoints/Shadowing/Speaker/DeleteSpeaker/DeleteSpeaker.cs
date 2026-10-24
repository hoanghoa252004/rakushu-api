using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Shadowing.Speakers.DeleteSpeaker;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.DeleteSpeaker;

internal sealed class DeleteSpeaker : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSpeakerEndpoints()
			.MapDelete("/{speakerId:guid}", async (
				[FromRoute] Guid speakerId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteSpeakerCommand(speakerId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteSpeaker")
			.WithSummary("Curator")
			.WithDescription("Deletes a speaker that is not used by any transcript.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
