using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Subscription.Feature;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Feature.ChangeFeatureStatus;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Feature.ChangeFeatureStatus;

internal sealed class ChangeFeatureStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFeatureEndpoints()
			// 1. Endpoint
			.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangeFeatureStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new ChangeFeatureStatusCommand(id, dto.Status);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("ChangeFeatureStatus")
			.WithDescription("Change feature status.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record ChangeFeatureStatusRequestDto(string Status);
