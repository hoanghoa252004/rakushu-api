using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Plan.UpdatePlan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Plan.UpdatePlan;

internal sealed class UpdatePlan : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdatePlanRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdatePlanCommand(
					id,
					dto.Name,
					dto.Price,
					dto.Currency,
					dto.BillingCycle,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithTags("Plan")
			.WithName("UpdatePlan")
			.WithDescription("Updates an existing plan with the provided details.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdatePlanRequestDto(
	string Name,
	decimal Price,
	string Currency,
	string BillingCycle,
	string? Description = null
);
