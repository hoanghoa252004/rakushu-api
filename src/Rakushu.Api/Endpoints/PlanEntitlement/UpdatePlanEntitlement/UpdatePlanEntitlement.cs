using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.PlanEntitlement.UpdatePlanEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.PlanEntitlement.UpdatePlanEntitlement;

internal sealed class UpdatePlanEntitlement : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEntitlementEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid planId,
				[FromRoute] Guid id,
				[FromBody] UpdatePlanEntitlementRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdatePlanEntitlementCommand(
					planId,
					id,
					dto.IsEnabled,
					dto.LimitValue,
					dto.LimitUnit,
					dto.LimitPeriod
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdatePlanEntitlement")
			.WithDescription("Updates a feature entitlement for the specified subscription plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdatePlanEntitlementRequestDto(
	bool IsEnabled = true,
	int LimitValue = 200,
	string LimitUnit = "Request",
	string LimitPeriod = "Month"
);
