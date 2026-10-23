using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.CreateContentKnowledgeCollection;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.DeleteContentKnowledgeCollection;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.GetContentKnowledgeCollectionById;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.GetContentKnowledgeCollections;
using Rakushu.Application.Usecases.Knowledge.ContentKnowledgeCollection.UpdateContentKnowledgeCollection;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.ContentKnowledgeCollection;

internal sealed class GetContentKnowledgeCollections : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentKnowledgeCollectionEndpoints()
			.MapGet("/", async (
				ISender sender,
				CancellationToken cancellationToken,
				int pageNumber = 1,
				int pageSize = 10,
				Guid? contentCategoryId = null,
				string? searchTerm = null,
				string? active = null,
				string? type = null
				) =>
			{
				var result = await sender.Send(new GetContentKnowledgeCollectionsQuery(
					pageNumber, pageSize, contentCategoryId, searchTerm, active, type), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetContentKnowledgeCollections")
			.WithSummary("Learner (active only), Curator")
			.WithDescription("Retrieves content knowledge collections. Learners only receive active records.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<ContentKnowledgeCollectionDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class GetContentKnowledgeCollectionById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentKnowledgeCollectionEndpoints()
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetContentKnowledgeCollectionByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetContentKnowledgeCollectionById")
			.WithSummary("Learner (active only), Curator")
			.WithDescription("Retrieves a content knowledge collection by ID.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<ContentKnowledgeCollectionDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class CreateContentKnowledgeCollection : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentKnowledgeCollectionEndpoints()
			.MapPost("/", async (
				[FromBody] CreateContentKnowledgeCollectionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new CreateContentKnowledgeCollectionCommand(
					dto.ContentCategoryId, dto.KnowledgeCollectionId, dto.Priority), cancellationToken);
				return result.MatchCreated("GetContentKnowledgeCollectionById", id => new { id });
			})
			.WithName("CreateContentKnowledgeCollection")
			.WithSummary("Curator")
			.WithDescription("Attach a knowledge collection to a content category (inactive by default). Priority must continue the existing sequence.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class UpdateContentKnowledgeCollection : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentKnowledgeCollectionEndpoints()
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateContentKnowledgeCollectionRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new UpdateContentKnowledgeCollectionCommand(id, dto.Priority, dto.IsActive), cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateContentKnowledgeCollection")
			.WithSummary("Curator")
			.WithDescription("Update priority and active state of a content knowledge collection.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed class DeleteContentKnowledgeCollection : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentKnowledgeCollectionEndpoints()
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteContentKnowledgeCollectionCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteContentKnowledgeCollection")
			.WithSummary("Curator")
			.WithDescription("Detach a knowledge collection from a content category and recalculate priorities.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateContentKnowledgeCollectionRequestDto(Guid ContentCategoryId, Guid KnowledgeCollectionId, int Priority);

internal record UpdateContentKnowledgeCollectionRequestDto(int Priority, bool IsActive);
