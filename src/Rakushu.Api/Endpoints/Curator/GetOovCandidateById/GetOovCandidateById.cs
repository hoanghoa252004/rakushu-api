using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curator.Oov.Common;
using Rakushu.Application.Usecases.Curator.Oov.GetOovCandidateById;
using Rakushu.Domain.Entities.Role;
using System;
using System.Threading;

namespace Rakushu.Api.Endpoints.Curator.GetOovCandidateById;

internal sealed class GetOovCandidateById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorEndpoints()
			.MapGet("/{id:guid}", async (
				Guid id,
				ISender sender,
				CancellationToken cancellationToken
			) =>
			{
				var query = new GetOovCandidateByIdQuery(id);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetOovCandidateById")
			.WithDescription("Retrieves detailed information and review history of an OOV candidate.")
			.RequireAuthorization(policy => policy.RequireRole(
				RoleCodes.LinguisticCurator,
				RoleCodes.SystemAdministrator
			))
			.Produces<OovCandidateDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
