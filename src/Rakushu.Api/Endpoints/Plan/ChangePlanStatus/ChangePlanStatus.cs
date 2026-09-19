using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Admin;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.ChangeUserStatus;
using Rakushu.Application.Usecases.Plan.ChangePlanStatus;
using Rakushu.Domain.Entities.Plan;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Api.Endpoints.Plan.ChangePlanStatus;

internal sealed class ChangePlanStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPlanEndpoints()
			// 1. Endpoint
			.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ChangePlanStatusRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new ChangePlanStatusCommand(id, dto.Status);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("ChangePlanStatus")
			.WithDescription("Change plan status.")
			// 3. Authentication & Authorization: already configure in MapAdminEndpoints()
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record ChangePlanStatusRequestDto(PlanStatus Status);
