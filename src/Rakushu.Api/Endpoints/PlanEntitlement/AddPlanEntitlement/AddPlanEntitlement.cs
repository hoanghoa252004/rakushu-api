using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.PlanEntitlement.AddPlanEntitlement;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.PlanEntitlement.AddPlanEntitlement;

internal sealed class AddPlanEntitlement : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEntitlementEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromRoute] Guid planId,
				[FromBody] AddPlanEntitlementRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new AddPlanEntitlementCommand(
					planId,
					dto.FeatureId,
					dto.IsEnabled,
					dto.LimitValue,
					dto.LimitUnit,
					dto.LimitPeriod
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetPlanEntitlements", _ => new { planId });
			})
			// 2. Description
			.WithName("AddPlanEntitlement")
			.WithDescription("Adds a feature entitlement to the specified subscription plan.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record AddPlanEntitlementRequestDto(
	Guid FeatureId,
	bool IsEnabled = true,
	int LimitValue = 100,
	string LimitUnit = "Request",
	string LimitPeriod = "Month"
);
