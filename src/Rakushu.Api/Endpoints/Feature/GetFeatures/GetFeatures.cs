using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Feature.GetFeatures;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Feature.GetFeatures;

internal sealed class GetFeatures : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFeatureEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? searchTerm = null,
				[FromQuery] string? status = null,
				ISender sender = default!,
				CancellationToken cancellationToken = default
				) =>
			{
				var query = new GetFeaturesQuery(
					pagination.PageNumber,
					pagination.PageSize,
					searchTerm,
					status
				);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetFeatures")
			.WithDescription("Retrieves a paginated list of features with optional filtering.")
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces<PaginatedList<FeatureDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}


