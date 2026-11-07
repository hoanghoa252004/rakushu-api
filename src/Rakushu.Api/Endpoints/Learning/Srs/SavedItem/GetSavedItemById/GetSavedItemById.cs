using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.GetSavedItemById;

internal sealed class GetSavedItemById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsSavedItemEndpoints()
			.MapGet("/{id:guid}", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSavedItemByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSavedItemById")
			.WithSummary("Learner")
			.WithDescription("Retrieves pure vocabulary definitions and dynamic multi-video contexts where this term appears across lesson videos.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SavedItemDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
