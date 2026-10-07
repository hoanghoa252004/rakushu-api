using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Entitlement.CreateEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Entitlement.CreateEntitlement;

internal sealed class CreateEntitlement : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapEntitlementEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromRoute] Guid planId,
				[FromBody] AddEntitlementRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateEntitlementCommand(
					planId,
					dto.FeatureId,
					dto.IsEnabled,
					dto.LimitUnit,
					dto.LimitValue,
					dto.LimitPeriod
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetEntitlements", id => new { id });
			})
			// 2. Description
			.WithName("CreateEntitlement")
			.WithSummary("Admin")
			.WithDescription("Creates a feature entitlement for the specified subscription plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record AddEntitlementRequestDto(
	Guid FeatureId,
	bool IsEnabled,
	string LimitUnit,
	int LimitValue,
	string LimitPeriod
);
