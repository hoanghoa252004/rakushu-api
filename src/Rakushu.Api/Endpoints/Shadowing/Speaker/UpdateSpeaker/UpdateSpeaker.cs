using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Shadowing.Speakers.UpdateSpeaker;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.UpdateSpeaker;

internal sealed class UpdateSpeaker : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSpeakerEndpoints()
			.MapPut("/{speakerId:guid}", async (
				[FromRoute] Guid speakerId,
				[FromBody] UpdateSpeakerRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new UpdateSpeakerCommand(
					speakerId, request.Name, request.JapaneseName,
					request.AvatarKey, request.Description, request.ThemeColor);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateSpeaker")
			.WithSummary("Curator")
			.WithDescription("Fully updates a speaker.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateSpeakerRequestDto(
	string Name,
	string JapaneseName,
	string? AvatarKey,
	string? Description,
	string ThemeColor);