using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.CreateBankQuestion;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.CreateBankQuestion;

internal sealed class CreateBankQuestion : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsBankQuestionEndpoints()
			.MapPost("", async (
				[FromBody] CreateBankQuestionRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateBankQuestionCommand(
					request.KnowledgeMeaningId,
					request.QuestionTypeId,
					request.Prompt,
					request.Choices,
					request.CorrectChoiceId,
					request.Explanation,
					request.Difficulty,
					request.IsActive);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetBankQuestionById", id => new { id });
			})
			.WithName("CreateBankQuestion")
			.WithSummary("Create a new bank question")
			.WithDescription("Allows linguistic curators and admins to add a new question to the SRS Question Bank.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateBankQuestionRequest(
	Guid KnowledgeMeaningId,
	Guid QuestionTypeId,
	string Prompt,
	IReadOnlyList<BankQuestionChoiceDto> Choices,
	string CorrectChoiceId,
	string? Explanation = null,
	string Difficulty = "Medium",
	bool IsActive = true);
