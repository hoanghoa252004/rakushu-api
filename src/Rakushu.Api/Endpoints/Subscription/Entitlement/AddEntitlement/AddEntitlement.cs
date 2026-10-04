using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Subscription.Entitlement.AddEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Subscription.Entitlement.AddEntitlement;

internal sealed class AddEntitlement : IEndpoint
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
				var command = new AddEntitlementCommand(
					planId,
					dto.FeatureId,
					dto.IsEnabled,
					dto.LimitValue,
					dto.LimitUnit,
					dto.LimitPeriod
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetEntitlements", _ => new { planId });
			})
			// 2. Description
			.WithName("AddEntitlement")
			.WithDescription("Adds a feature entitlement to the specified subscription plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record AddEntitlementRequestDto(
	Guid FeatureId,
	bool IsEnabled = true,
	int LimitValue = 100,
	string LimitUnit = "Request",
	string LimitPeriod = "Month"
);
