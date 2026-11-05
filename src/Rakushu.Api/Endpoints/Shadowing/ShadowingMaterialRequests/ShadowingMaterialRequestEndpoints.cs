using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.ChangeShadowingMaterialRequestStatus;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.CreateShadowingMaterialRequest;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.DeleteShadowingMaterialRequest;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.GetShadowingMaterialRequestById;
using Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests.GetShadowingMaterialRequests;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Api.Endpoints.Shadowing.ShadowingMaterialRequests;

internal static class ShadowingMaterialRequestEndpointGroupExtension
{
	internal static RouteGroupBuilder MapShadowingMaterialRequestEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/videos/{videoId:guid}/shadowing-material-requests")
			.WithTags("ShadowingMaterialRequest")
			.WithGroupName("shadowing");
	}

	internal static RouteGroupBuilder MapAllShadowingMaterialRequestEndpoints(this IEndpointRouteBuilder app)
	{
		return app.MapGroup("/api/shadowing-material-requests")
			.WithTags("ShadowingMaterialRequest")
			.WithGroupName("shadowing");
	}
}

internal sealed class CreateShadowingMaterialRequestEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialRequestEndpoints()
			.MapPost("/", async (
				[FromRoute] Guid videoId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new CreateShadowingMaterialRequestCommand(videoId), cancellationToken);
				return result.MatchCreated("GetShadowingMaterialRequestById", id => new { videoId, requestId = id });
			})
			.WithName("CreateShadowingMaterialRequest")
			.WithSummary("Learner")
			.WithDescription("Learner requests a shadowing material for a video. The video must be Ready.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingMaterialRequestsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAllShadowingMaterialRequestEndpoints()
			.MapGet("/", async (
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] Guid? videoId = null,
				[FromQuery] Guid? userId = null,
				[FromQuery] ShadowingMaterialRequestStatus? status = null,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetShadowingMaterialRequestsQuery(pageNumber, pageSize, videoId, userId, status), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingMaterialRequests")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets requests with paging, filter by videoId, userId, status. Learner only sees own requests (userId is ignored); Curator sees all.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<ShadowingMaterialRequestDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetShadowingMaterialRequestByIdEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialRequestEndpoints()
			.MapGet("/{requestId:guid}", async (
				[FromRoute] Guid requestId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetShadowingMaterialRequestByIdQuery(requestId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetShadowingMaterialRequestById")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets a request by id. Learner can only get own request.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<ShadowingMaterialRequestDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class ChangeShadowingMaterialRequestStatusEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialRequestEndpoints()
			.MapPatch("/{requestId:guid}/status", async (
				[FromRoute] Guid requestId,
				[FromBody] ChangeShadowingMaterialRequestStatusRequestDto request,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new ChangeShadowingMaterialRequestStatusCommand(requestId, request.Status, request.RejectReason),
					cancellationToken);
				return result.MatchOk();
			})
			.WithName("ChangeShadowingMaterialRequestStatus")
			.WithSummary("Curator")
			.WithDescription("Approves (Processing, creates a RequestedByLearner material) or rejects (Rejected, rejectReason required) a Pending request.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record ChangeShadowingMaterialRequestStatusRequestDto(
	ShadowingMaterialRequestStatus Status,
	string? RejectReason);

internal sealed class DeleteShadowingMaterialRequestEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapShadowingMaterialRequestEndpoints()
			.MapDelete("/{requestId:guid}", async (
				[FromRoute] Guid requestId,
				ISender sender,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new DeleteShadowingMaterialRequestCommand(requestId), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteShadowingMaterialRequest")
			.WithSummary("Learner, Curator")
			.WithDescription("Deletes a request only while it is Pending. Learner can only delete own request.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
