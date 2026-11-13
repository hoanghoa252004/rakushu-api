using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingAttempts;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.ShadowingAttempts;

public sealed record CreateAttemptDetailRequestDto(Guid RoleLineId, string? AudioKey, long? DurationMs, bool? IsSkipped);

internal sealed class ShadowingAttemptEndpoints : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		var sessions = app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing");

		sessions.MapPatch("/{sessionId:guid}/cancel", async (
				[FromRoute] Guid sessionId, ISender sender, CancellationToken ct = default) =>
			{
				var result = await sender.Send(new CancelShadowingSessionCommand(sessionId), ct);
				return result.MatchOk();
			})
			.WithName("CancelShadowingSession")
			.WithSummary("Learner (host)")
			.WithDescription("Cancels a Scheduled session and revokes its pending invitations.")
			.RequireAuthorization(p => p.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);

		sessions.MapPatch("/{sessionId:guid}/start", async (
				[FromRoute] Guid sessionId, ISender sender, CancellationToken ct = default) =>
			{
				var result = await sender.Send(new StartShadowingSessionCommand(sessionId), ct);
				return result.MatchOk();
			})
			.WithName("StartShadowingSession")
			.WithSummary("Learner (host)")
			.WithDescription("Starts the session when members match the material's roles and all have chosen a role. Creates an InProgress attempt for every member.")
			.RequireAuthorization(p => p.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict);

		sessions.MapGet("/{sessionId:guid}/progress", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken ct = default) =>
			{
				var result = await sender.Send(new GetSessionProgressQuery(sessionId, pageNumber, pageSize), ct);
				return result.MatchOk();
			})
			.WithName("GetShadowingSessionProgress")
			.Produces<SessionProgressDto>(StatusCodes.Status200OK)
			.WithSummary("Learner (member)")
			.WithDescription("Gets, with paging, every role line of the session in order with its state (Completed, Skipped, AttemptCancelled, Pending), owner member/attempt status and the owner's attempt detail (audio url) so later members can listen to earlier lines.")
			.RequireAuthorization(p => p.RequireRole(RoleCodes.Learner))
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);

		var attempts = app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/attempts")
			.WithTags("ShadowingAttempt").WithGroupName("shadowing");

		attempts.MapPatch("/{attemptId:guid}/cancel", async (
				[FromRoute] Guid sessionId, [FromRoute] Guid attemptId, ISender sender, CancellationToken ct = default) =>
			{
				var result = await sender.Send(new CancelShadowingAttemptCommand(attemptId), ct);
				return result.MatchOk();
			})
			.WithName("CancelShadowingAttempt")
			.WithSummary("Learner (member)")
			.WithDescription("Cancels an attempt. The owner can cancel their own attempt; the session host can cancel a member's attempt only when skipped details reach at least half of that member's role lines. Attempt and session must be InProgress.")
			.RequireAuthorization(p => p.RequireRole(RoleCodes.Learner));

		attempts.MapPost("/{attemptId:guid}/details", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid attemptId,
				[FromBody] CreateAttemptDetailRequestDto request,
				ISender sender,
				CancellationToken ct = default) =>
			{
				var result = await sender.Send(new CreateAttemptDetailCommand(
					sessionId, attemptId, request.RoleLineId, request.AudioKey, request.DurationMs, request.IsSkipped), ct);
				return result.MatchOk();
			})
			.WithName("CreateShadowingAttemptDetail")
			.Produces<Guid>(StatusCodes.Status200OK)
			.WithSummary("Learner (member)")
			.WithDescription("Creates the detail of a role line, or skips it when isSkipped = true. The previous role line of the material must already be done. When the learner has a detail for every line of their role, the attempt becomes Completed automatically. Details cannot be updated or deleted. The session host may also create an isSkipped = true detail for a member's line (e.g. the member is stalling or disconnected).")
			.RequireAuthorization(p => p.RequireRole(RoleCodes.Learner));

		Action<Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder> readPolicy = p =>
			p.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator);

		attempts.MapGet("/{attemptId:guid}", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid attemptId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] bool? isSkipped = null,
				CancellationToken ct = default) =>
			{
				var result = await sender.Send(new GetShadowingAttemptQuery(sessionId, attemptId, pageNumber, pageSize, isSkipped), ct);
				return result.MatchOk();
			})
			.WithName("GetShadowingAttempt")
			.Produces<AttemptViewDto>(StatusCodes.Status200OK)
			.WithSummary("Learner (owner), Curator")
			.WithDescription("Gets an attempt with paged details (filter isSkipped) and the role lines missing because the attempt was cancelled.")
			.RequireAuthorization(readPolicy)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);

		attempts.MapGet("/", async (
				[FromRoute] Guid sessionId,
				ISender sender,
				CancellationToken ct = default) =>
			{
				var result = await sender.Send(new GetShadowingAttemptsQuery(sessionId), ct);
				return result.MatchOk();
			})
			.WithName("GetShadowingAttempts")
			.Produces<AttemptListDto>(StatusCodes.Status200OK)
			.WithSummary("Learner (member, own attempts), Curator (all)")
			.WithDescription("Lists attempts of a session with session progress (1.0 = 100%, skipped and cancelled count as done) and material role summary.")
			.RequireAuthorization(readPolicy)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound);
	}
}
