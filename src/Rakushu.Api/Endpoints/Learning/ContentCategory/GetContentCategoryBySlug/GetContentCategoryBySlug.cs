using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryBySlug;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.GetContentCategoryBySlug;

internal sealed class GetContentCategoryBySlug : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapGet("/{slug}", async (
				[FromRoute] string slug,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetContentCategoryBySlugQuery(slug);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetContentCategoryBySlug")
			.WithSummary("Allow Anonymous")
			.WithDescription("Retrieves a specific content category by its slug.")
			.AllowAnonymous()
			// 4. Response
			.Produces<ContentCategoryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
