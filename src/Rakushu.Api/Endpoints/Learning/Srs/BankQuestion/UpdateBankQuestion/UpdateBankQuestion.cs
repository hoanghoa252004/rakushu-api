using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.UpdateBankQuestion;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.UpdateBankQuestion;

internal sealed class UpdateBankQuestion : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsBankQuestionEndpoints()
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateBankQuestionRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new UpdateBankQuestionCommand(
					id,
					request.Prompt,
					request.Choices,
					request.CorrectChoiceId,
					request.Explanation,
					request.Difficulty);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateBankQuestion")
			.WithSummary("Update bank question details")
			.WithDescription("Allows linguistic curators and admins to update the prompt, choices, correct choice, explanation, or difficulty of a bank question.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateBankQuestionRequest(
	string Prompt,
	IReadOnlyList<BankQuestionChoiceDto> Choices,
	string CorrectChoiceId,
	string? Explanation = null,
	string Difficulty = "Medium");
