using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Plan.CreatePlan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Plan.CreatePlan;

internal sealed class CreatePlan : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreatePlanRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreatePlanCommand(
					dto.Code,
					dto.Name,
					dto.Price,
					dto.Currency,
					dto.BillingCycle,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetPlanById", planId => new { id = planId });
			})
			// 2. Description
			.WithName("CreatePlan")
			.WithDescription("Creates a new subscription plan with the specified details.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreatePlanRequestDto(
	string Code = "PLAN_NAME_A",
	string Name = "Plan Name A",
	decimal Price = 50000,
	string Currency = "VND",
	string BillingCycle = "Monthly",
	string? Description = null
);
