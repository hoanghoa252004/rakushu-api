using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Application.Usecases.Curator.Oov.GetOovCandidates;
using Rakushu.Domain.Entities.Role;
using System.Threading;

namespace Rakushu.Api.Endpoints.Curator.GetOovCandidates;

internal sealed class GetOovCandidates : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorEndpoints()
			.MapGet("/", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? searchTerm = null,
				[FromQuery] string? status = null,
				ISender sender = default!,
				CancellationToken cancellationToken = default
			) =>
			{
				var query = new GetOovCandidatesQuery(
					pagination.PageNumber,
					pagination.PageSize,
					searchTerm,
					status
				);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetOovCandidates")
			.WithDescription("Retrieves a paginated list of OOV candidates awaiting curator review.")
			.RequireAuthorization(policy => policy.RequireRole(
				DefaultSystemRoles.LinguisticCurator.ToString(),
				DefaultSystemRoles.SystemAdministrator.ToString()
			))
			.Produces<PaginatedList<OovCandidateDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
