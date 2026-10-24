using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Shadowing.Speakers;
using Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakersByVideo;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.GetSpeakersByVideo;

internal sealed class GetSpeakersByVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/videos/{videoId:guid}/speakers", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetSpeakersByVideoQuery(videoId), cancellationToken);
				return result.MatchOk();
			})
			.WithGroupName("shadowing")
			.WithTags("Speaker")
			.WithName("GetVideoSpeakers")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets the distinct speakers that appear in the segments of a video transcript.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<IReadOnlyList<SpeakerDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
