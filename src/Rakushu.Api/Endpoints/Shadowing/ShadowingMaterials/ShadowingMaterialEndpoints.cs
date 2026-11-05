using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.ChangeShadowingMaterialStatus;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.CreateShadowingMaterial;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.DeleteShadowingMaterial;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterialById;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterials.GetShadowingMaterials;
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

internal sealed class CreateShadowingMaterialEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapPost("/", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new CreateShadowingMaterialCommand(videoId), cancellationToken);
				return result.MatchCreated("GetShadowingMaterialById", id => new { videoId, materialId = id });
			})
			.WithName("CreateShadowingMaterial")
			.WithSummary("Curator")
			.WithDescription("Creates a Curated shadowing material (not linked to any request) for a Ready video. Status starts as Processing.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
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
				[FromQuery] Guid? requestId = null,
				[FromQuery] MaterialType? type = null,
				[FromQuery] ShadowingMaterialStatus? status = null,
				[FromQuery] int linePageNumber = 1,
				[FromQuery] int linePageSize = 20,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetShadowingMaterialsQuery(pageNumber, pageSize, videoId, requestId, type, status, linePageNumber, linePageSize), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingMaterials")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets materials of a video with paging, filter by requestId, type, status. Learner only sees Ready materials (status filter ignored); Curator sees all.")
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

internal sealed class ChangeShadowingMaterialStatusEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapPatch("/{materialId:guid}/status", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid materialId,
				[FromBody] ChangeShadowingMaterialStatusRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new ChangeShadowingMaterialStatusCommand(materialId, request.Status), cancellationToken);
				return result.MatchOk();
			})
			.WithName("ChangeShadowingMaterialStatus")
			.WithSummary("Curator")
			.WithDescription("Changes material status following Processing -> Reviewing -> Ready -> Archived. Setting Ready also completes the linked request.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record ChangeShadowingMaterialStatusRequestDto(ShadowingMaterialStatus Status);

internal sealed class DeleteShadowingMaterialEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialEndpoints()
			.MapDelete("/{materialId:guid}", async (
				[FromRoute] Guid videoId,
				[FromRoute] Guid materialId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingMaterialCommand(materialId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingMaterial")
			.WithSummary("Curator")
			.WithDescription("Deletes a material only while it is Processing.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
