using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.ToggleBankQuestionStatus;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.ToggleBankQuestionStatus;

internal sealed class ToggleBankQuestionStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsBankQuestionEndpoints()
			.MapPatch("/{id:guid}/status", async (
				[FromRoute] Guid id,
				[FromBody] ToggleBankQuestionStatusRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new ToggleBankQuestionStatusCommand(id, request.IsActive);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("ToggleBankQuestionStatus")
			.WithSummary("Activate or deactivate a bank question")
			.WithDescription("Allows linguistic curators and admins to toggle the active status of a bank question.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record ToggleBankQuestionStatusRequest(bool IsActive);
