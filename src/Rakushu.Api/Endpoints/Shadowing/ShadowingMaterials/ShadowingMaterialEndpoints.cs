using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.CreateShadowingMaterial;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.DeleteShadowingMaterial;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterialById;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterials;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.UpdateShadowingMaterial;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines.GetShadowingRoleLines;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles.GetShadowingRoles;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Api.Endpoints.Shadowing.ShadowingMaterials;

internal static class ShadowingMaterialEndpointGroupExtension
{
	internal static RouteGroupBuilder MapShadowingMaterialEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos/{videoId:guid}/shadowing-materials")
			.WithTags("ShadowingMaterial")
			.WithGroupName("shadowing");
	}
}

internal static class ShadowingMaterialByIdEndpointGroupExtension
{
	internal static RouteGroupBuilder MapShadowingMaterialByIdEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/shadowing-materials")
			.WithTags("ShadowingMaterial")
			.WithGroupName("shadowing");
	}
}

internal sealed class CreateShadowingMaterialEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapPost("/", async (
				[FromRoute] Guid videoId,
				[FromBody] CreateShadowingMaterialRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new CreateShadowingMaterialCommand(
					videoId, request.Name, request.StartTranscriptSegmentId, request.EndTranscriptSegmentId, request.SpeakerIds), cancellationToken);
				return result.MatchCreated("GetShadowingMaterialById", id => new { videoId, materialId = id });
			})
			.WithName("CreateShadowingMaterial")
			.WithSummary("Learner, Curator")
			.WithDescription("Creates a shadowing material for a transcript segment range with the selected transcript speakers (one role per speaker). Status starts as Processing. A learner can only have one Processing material at a time.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingMaterialsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapGet("/", async (
				[FromRoute] Guid videoId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? status = null,
				[FromQuery] int linePageNumber = 1,
				[FromQuery] int linePageSize = 20,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetShadowingMaterialsQuery(pageNumber, pageSize, videoId, status, linePageNumber, linePageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingMaterials")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets materials of a video with paging, filter by status. Learner sees all materials of their own video, otherwise only Ready materials; Curator sees all.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<ShadowingMaterialDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingMaterialByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapGet("/{materialId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid materialId,
				ISender sender,
				[FromQuery] int linePageNumber = 1,
				[FromQuery] int linePageSize = 20,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingMaterialByIdQuery(materialId, linePageNumber, linePageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingMaterialById")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets a material by id. Learner only gets Ready materials.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<ShadowingMaterialDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateShadowingMaterialRequestDto(
	string Name,
	Guid StartTranscriptSegmentId,
	Guid EndTranscriptSegmentId,
	IReadOnlyList<Guid> SpeakerIds);

public sealed record UpdateShadowingMaterialRequestDto(
	string Name,
	Guid StartTranscriptSegmentId,
	Guid EndTranscriptSegmentId,
	IReadOnlyList<Guid> SpeakerIds);

internal sealed class UpdateShadowingMaterialEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialByIdEndpoints()
			.MapPut("/{materialId:guid}", async (
				[FromRoute] Guid materialId,
				[FromBody] UpdateShadowingMaterialRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new UpdateShadowingMaterialCommand(
					materialId, request.Name, request.StartTranscriptSegmentId, request.EndTranscriptSegmentId, request.SpeakerIds), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateShadowingMaterial")
			.WithSummary("Learner, Curator")
			.WithDescription("Updates name, segment range and transcript speakers. Range/speakers can only change while no session exists; roles and lines are rebuilt and the material returns to Processing.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingRolesEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-materials/{materialId:guid}/roles")
			.WithTags("ShadowingMaterial").WithGroupName("shadowing")
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

internal sealed class GetShadowingRoleLinesEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGroup("/api/shadowing-materials/{materialId:guid}/roles/{shadowingRoleId:guid}/lines")
			.WithTags("ShadowingMaterial").WithGroupName("shadowing")
			.MapGet("/", async (
				[FromRoute] Guid materialId,
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

internal sealed class DeleteShadowingMaterialEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialByIdEndpoints()
			.MapDelete("/{materialId:guid}", async (
				[FromRoute] Guid materialId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingMaterialCommand(materialId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingMaterial")
			.WithSummary("Learner, Curator")
			.WithDescription("Deletes a material (with its roles and lines) only when no session has been created from it.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
