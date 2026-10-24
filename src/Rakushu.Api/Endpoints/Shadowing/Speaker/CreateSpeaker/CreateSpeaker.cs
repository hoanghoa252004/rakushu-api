using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Shadowing.Speakers.CreateSpeaker;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.CreateSpeaker;

internal sealed class CreateSpeaker : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSpeakerEndpoints()
			.MapPost("/", async (
				[FromBody] CreateSpeakerRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var command = new CreateSpeakerCommand(
					request.Code, request.Name, request.JapaneseName,
					request.AvatarKey, request.Description, request.ThemeColor);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetSpeakerById", id => new { speakerId = id });
			})
			.WithName("CreateSpeaker")
			.WithSummary("Curator")
			.WithDescription("Creates a speaker.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateSpeakerRequestDto(
	string Code,
	string Name,
	string JapaneseName,
	string? AvatarKey,
	string? Description,
	string ThemeColor);
