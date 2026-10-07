using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Subscription.Feature;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.GetContentCategoryById;

internal sealed class GetContentCategoryById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetContentCategoryByIdQuery(id);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetContentCategoryById")
			.WithSummary("Admin")
			.WithDescription("Retrieves a specific content category by its ID.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<ContentCategoryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
