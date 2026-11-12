using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.TranscriptSpeakers;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.TranscriptSpeakers;

internal sealed class CreateTranscriptSpeakerEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSpeakerEndpoints()
			.MapPost("/speakers", async (
				[FromRoute] Guid videoId,
				[FromBody] CreateTranscriptSpeakerRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new CreateTranscriptSpeakerCommand(videoId, request.SpeakerId, request.Label), cancellationToken);
				return result.MatchOk();
			})
			.WithName("CreateTranscriptSpeaker")
			.WithSummary("Curator")
			.WithDescription("Adds a speaker to the transcript of a video. A speaker can only be added once per transcript.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<Guid>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);
	}
}

internal sealed class GetTranscriptSpeakersEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSpeakerEndpoints()
			.MapGet("/speakers", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptSpeakersQuery(videoId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscriptSpeakers")
			.WithSummary("Curator")
			.WithDescription("Retrieves the speakers of a transcript, including speaker information.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<IReadOnlyList<TranscriptSpeakerDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

internal sealed class GetTranscriptSpeakerByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSpeakerEndpoints()
			.MapGet("/speakers/{transcriptSpeakerId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid transcriptSpeakerId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetTranscriptSpeakerByIdQuery(videoId, transcriptSpeakerId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetTranscriptSpeakerById")
			.WithSummary("Curator")
			.WithDescription("Retrieves a transcript speaker, including speaker information.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<TranscriptSpeakerDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

internal sealed class UpdateTranscriptSpeakerEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSpeakerEndpoints()
			.MapPut("/speakers/{transcriptSpeakerId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid transcriptSpeakerId,
				[FromBody] UpdateTranscriptSpeakerRequest request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new UpdateTranscriptSpeakerCommand(videoId, transcriptSpeakerId, request.SpeakerId, request.Label), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateTranscriptSpeaker")
			.WithSummary("Curator")
			.WithDescription("Updates the label and speaker of a transcript speaker.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);
	}
}

internal sealed class DeleteTranscriptSpeakerEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptSpeakerEndpoints()
			.MapDelete("/speakers/{transcriptSpeakerId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid transcriptSpeakerId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteTranscriptSpeakerCommand(videoId, transcriptSpeakerId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteTranscriptSpeaker")
			.WithSummary("Curator")
			.WithDescription("Deletes a transcript speaker. Segments using it will have their speaker cleared.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}
}

public sealed record CreateTranscriptSpeakerRequest(Guid SpeakerId, string Label);

public sealed record UpdateTranscriptSpeakerRequest(Guid SpeakerId, string Label);
