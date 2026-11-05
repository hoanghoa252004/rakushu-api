using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.CreateShadowingRoleLine;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.DeleteShadowingRoleLine;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.GetShadowingRoleLines;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.UpdateShadowingRoleLine;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles.CreateShadowingRole;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles.DeleteShadowingRole;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles.GetShadowingRoles;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles.UpdateShadowingRole;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.ShadowingRoles;

internal sealed class GetShadowingRolesEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-materials/{materialId:guid}/roles")
			.WithTags("ShadowingRole").WithGroupName("shadowing")
			.MapGet("/", async ([FromRoute] Guid materialId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingRolesQuery(materialId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingRoles")
			.WithSummary("Curator")
			.WithDescription("Gets all roles of a material ordered by sortOrder.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<IReadOnlyList<ShadowingRoleDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class CreateShadowingRoleEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-materials/{materialId:guid}/roles")
			.WithTags("ShadowingRole").WithGroupName("shadowing")
			.MapPost("/", async (
				[FromRoute] Guid materialId,
				[FromBody] ShadowingRoleRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new CreateShadowingRoleCommand(materialId, request.SpeakerId, request.SortOrder), cancellationToken);
				return result.MatchOk();
			})
			.WithName("CreateShadowingRole")
			.WithSummary("Curator")
			.WithDescription("Creates a role in a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record ShadowingRoleRequestDto(Guid SpeakerId, int SortOrder);

internal sealed class UpdateShadowingRoleEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-roles")
			.WithTags("ShadowingRole").WithGroupName("shadowing")
			.MapPut("/{roleId:guid}", async (
				[FromRoute] Guid roleId,
				[FromBody] ShadowingRoleRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new UpdateShadowingRoleCommand(roleId, request.SpeakerId, request.SortOrder), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateShadowingRole")
			.WithSummary("Curator")
			.WithDescription("Updates a role of a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class DeleteShadowingRoleEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-roles")
			.WithTags("ShadowingRole").WithGroupName("shadowing")
			.MapDelete("/{roleId:guid}", async ([FromRoute] Guid roleId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingRoleCommand(roleId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingRole")
			.WithSummary("Curator")
			.WithDescription("Deletes a role (and its lines) of a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingRoleLinesEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-roles/{shadowingRoleId:guid}/lines")
			.WithTags("ShadowingRoleLine").WithGroupName("shadowing")
			.MapGet("/", async (
				[FromRoute] Guid shadowingRoleId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetShadowingRoleLinesQuery(shadowingRoleId, pageNumber, pageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingRoleLines")
			.WithSummary("Curator")
			.WithDescription("Gets role lines of a material with paging, filter by speakerId or roleId. AudioUrl is a presigned read URL.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<ShadowingRoleLineDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class CreateShadowingRoleLineEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-roles/{shadowingRoleId:guid}/lines")
			.WithTags("ShadowingRoleLine").WithGroupName("shadowing")
			.MapPost("/", async (
				[FromRoute] Guid shadowingRoleId,
				[FromBody] CreateShadowingRoleLineRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new CreateShadowingRoleLineCommand(
					shadowingRoleId, request.TranscriptSegmentIds), cancellationToken);
				return result.MatchOk();
			})
			.WithName("CreateShadowingRoleLine")
			.WithSummary("Curator")
			.WithDescription("Creates a role line in a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateShadowingRoleLineRequestDto(List<Guid> TranscriptSegmentIds);

public sealed record ShadowingRoleLineRequestDto(
	Guid RoleId,
	List<Guid> TranscriptSegmentIds);

internal sealed class UpdateShadowingRoleLineEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-role-lines")
			.WithTags("ShadowingRoleLine").WithGroupName("shadowing")
			.MapPut("/{lineId:guid}", async (
				[FromRoute] Guid lineId,
				[FromBody] ShadowingRoleLineRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new UpdateShadowingRoleLineCommand(
					lineId, request.RoleId, request.TranscriptSegmentIds), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateShadowingRoleLine")
			.WithSummary("Curator")
			.WithDescription("Updates a role line of a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class DeleteShadowingRoleLineEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-role-lines")
			.WithTags("ShadowingRoleLine").WithGroupName("shadowing")
			.MapDelete("/{lineId:guid}", async ([FromRoute] Guid lineId, ISender sender, CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingRoleLineCommand(lineId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingRoleLine")
			.WithSummary("Curator")
			.WithDescription("Deletes a role line of a material that is in Reviewing status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
