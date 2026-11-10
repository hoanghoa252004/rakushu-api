using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessionInvitations;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessionMembers;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions.CreateShadowingSession;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions.DeleteShadowingSession;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions.GetShadowingSession;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions.GetShadowingSessions;
using Rakushu.Application.Usecases.Shadowing.ShadowingSessions.UpdateShadowingSession;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingSession;
using Rakushu.Domain.Entities.Shadowing.ShadowingSessionInvitation;

namespace Rakushu.Api.Endpoints.Shadowing.ShadowingSessions;

public sealed record ShadowingSessionRequestDto(
	string Title,
	string Visibility,
	DateTimeOffset ScheduledStartAt);

public sealed record CreateShadowingSessionRequestDto(
	Guid MaterialId,
	string Title,
	string Visibility,
	DateTimeOffset ScheduledStartAt);

public sealed record CreateInvitationsRequestDto(List<Guid> LearnerIds);
public sealed record RespondInvitationRequestDto(string Status);
public sealed record ChooseRoleRequestDto(Guid RoleId);

internal sealed class CreateShadowingSessionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapPost("/", async (
				[FromBody] CreateShadowingSessionRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new CreateShadowingSessionCommand(
					request.MaterialId, request.Title, request.Visibility, request.ScheduledStartAt), cancellationToken);
				return result.MatchOk();
			})
			.WithName("CreateShadowingSession")
			.Produces<Guid>(StatusCodes.Status200OK)
			.WithSummary("Learner")
			.WithDescription("Creates a Scheduled session from a Ready material of the learner's own video or a curated video. The host is added as a member automatically.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class UpdateShadowingSessionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapPut("/{sessionId:guid}", async (
				[FromRoute] Guid sessionId,
				[FromBody] ShadowingSessionRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new UpdateShadowingSessionCommand(
					sessionId, request.Title, request.Visibility, request.ScheduledStartAt), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateShadowingSession")
			.WithSummary("Learner (host)")
			.WithDescription("Updates title, visibility and scheduledStartAt while the session is Scheduled.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class DeleteShadowingSessionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapDelete("/{sessionId:guid}", async ([FromRoute] Guid sessionId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingSessionCommand(sessionId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingSession")
			.WithSummary("Learner (host)")
			.WithDescription("Deletes a Scheduled session that has no member other than the host.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetMyShadowingSessionsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapGet("/me", async (
				ISender sender,
				[FromQuery] string? status = null,
				[FromQuery] string? visibility = null,
				[FromQuery] Guid? hostId = null,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetMyShadowingSessionsQuery(
					status, visibility, hostId, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyShadowingSessions")
			.Produces<PaginatedList<ShadowingSessionDto>>(StatusCodes.Status200OK)
			.WithSummary("Learner")
			.WithDescription("Gets sessions the current learner participates in, including material, video, host and members.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetAllShadowingSessionsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapGet("/", async (
				ISender sender,
				[FromQuery] string? status = null,
				[FromQuery] string? visibility = null,
				[FromQuery] Guid? hostId = null,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingSessionsQuery(
					status, visibility, hostId, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetAllShadowingSessions")
			.Produces<PaginatedList<ShadowingSessionDto>>(StatusCodes.Status200OK)
			.WithSummary("Learner & Curator")
			.WithDescription("Gets sessions with members, material, video and host. Curator sees all sessions (optional visibility filter); learner only gets Public sessions and cannot request Private.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.Learner))
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingSessionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions")
			.WithTags("ShadowingSession").WithGroupName("shadowing")
			.MapGet("/{sessionId:guid}", async ([FromRoute] Guid sessionId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingSessionQuery(sessionId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingSession")
			.Produces<ShadowingSessionDto>(StatusCodes.Status200OK)
			.WithSummary("Learner / Curator")
			.WithDescription("Gets a session detail. Learners need to be a member or the session must be public.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class JoinShadowingSessionEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/members")
			.WithTags("ShadowingSessionMember").WithGroupName("shadowing")
			.MapPost("/", async ([FromRoute] Guid sessionId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new JoinShadowingSessionCommand(sessionId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("JoinShadowingSession")
			.WithSummary("Learner")
			.WithDescription("Joins a public Scheduled session (JoinType = PublicJoin).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class RemoveShadowingSessionMemberEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/members")
			.WithTags("ShadowingSessionMember").WithGroupName("shadowing")
			.MapDelete("/{memberId:guid}", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid memberId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new RemoveShadowingSessionMemberCommand(sessionId, memberId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("RemoveShadowingSessionMember")
			.WithSummary("Learner (host)")
			.WithDescription("Removes a member from a Scheduled session. The host cannot be removed.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class ChooseShadowingSessionRoleEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/members")
			.WithTags("ShadowingSessionMember").WithGroupName("shadowing")
			.MapPut("/{memberId:guid}/role", async (
				[FromRoute] Guid sessionId,
				[FromRoute] Guid memberId,
				[FromBody] ChooseRoleRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new ChooseShadowingSessionRoleCommand(sessionId, memberId, request.RoleId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("ChooseShadowingSessionRole")
			.WithSummary("Learner (member)")
			.WithDescription("Member chooses a role of the session's material. A role can only be chosen by one member.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class CreateShadowingSessionInvitationsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/invitations")
			.WithTags("ShadowingSessionInvitation").WithGroupName("shadowing")
			.MapPost("/", async (
				[FromRoute] Guid sessionId,
				[FromBody] CreateInvitationsRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new CreateShadowingSessionInvitationsCommand(sessionId, request.LearnerIds), cancellationToken);
				return result.MatchOk();
			})
			.WithName("CreateShadowingSessionInvitations")
			.Produces<IReadOnlyList<Guid>>(StatusCodes.Status200OK)
			.WithSummary("Learner (host)")
			.WithDescription("Invites learners. ExpiredAt is 10 minutes before the session's scheduledStartAt.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetMyShadowingSessionInvitationsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-session-invitations")
			.WithTags("ShadowingSessionInvitation").WithGroupName("shadowing")
			.MapGet("/me", async (
				ISender sender,
				[FromQuery] string? status = null,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetMyShadowingSessionInvitationsQuery(status, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyShadowingSessionInvitations")
			.Produces<PaginatedList<ShadowingSessionInvitationDto>>(StatusCodes.Status200OK)
			.WithSummary("Learner")
			.WithDescription("Gets invitations received by the current learner.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class RespondShadowingSessionInvitationEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-session-invitations")
			.WithTags("ShadowingSessionInvitation").WithGroupName("shadowing")
			.MapPatch("/{invitationId:guid}", async (
				[FromRoute] Guid invitationId,
				[FromBody] RespondInvitationRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new RespondShadowingSessionInvitationCommand(invitationId, request.Status), cancellationToken);
				return result.MatchOk();
			})
			.WithName("RespondShadowingSessionInvitation")
			.WithSummary("Learner (invitee)")
			.WithDescription("Accepts or rejects a pending invitation. Accepting creates a member with JoinType = Invitation.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateInvitationRequestDto(Guid LearnerId);

internal sealed class GetShadowingSessionMembersEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-sessions/{sessionId:guid}/members")
			.WithTags("ShadowingSessionMember").WithGroupName("shadowing")
			.MapGet("/", async ([FromRoute] Guid sessionId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingSessionMembersQuery(sessionId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingSessionMembers")
			.WithSummary("Learner (member), Curator")
			.WithDescription("Gets the members of a session.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<IReadOnlyList<ShadowingSessionMemberDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class UpdateShadowingSessionInvitationEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-session-invitations")
			.WithTags("ShadowingSessionInvitation").WithGroupName("shadowing")
			.MapPut("/{invitationId:guid}", async (
				[FromRoute] Guid invitationId,
				[FromBody] UpdateInvitationRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new UpdateShadowingSessionInvitationCommand(invitationId, request.LearnerId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateShadowingSessionInvitation")
			.WithSummary("Learner (host)")
			.WithDescription("Changes the invited learner of an invitation that is still Pending.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class DeleteShadowingSessionInvitationEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-session-invitations")
			.WithTags("ShadowingSessionInvitation").WithGroupName("shadowing")
			.MapDelete("/{invitationId:guid}", async ([FromRoute] Guid invitationId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingSessionInvitationCommand(invitationId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingSessionInvitation")
			.WithSummary("Learner (host)")
			.WithDescription("Deletes an invitation that is still Pending.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
