using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Shadowing.Speakers;
using Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakerById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.GetSpeakerById;

internal sealed class GetSpeakerById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSpeakerEndpoints()
			.MapGet("/{speakerId:guid}", async (
				[FromRoute] Guid speakerId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetSpeakerByIdQuery(speakerId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSpeakerById")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets a speaker by id. AvatarUrl is a presigned read URL when the speaker has an avatar.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<SpeakerDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}


