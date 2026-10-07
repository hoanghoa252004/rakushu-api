using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategories;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.GetContentCategories;

internal sealed class GetContentCategories : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{

		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetContentCategoriesQuery();

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetContentCategories")
			.WithSummary("Allow Anonymous")
			.WithDescription("Retrieves all content categories.")
			.AllowAnonymous()
			// 4. Response
			.Produces<ContentCategoryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
