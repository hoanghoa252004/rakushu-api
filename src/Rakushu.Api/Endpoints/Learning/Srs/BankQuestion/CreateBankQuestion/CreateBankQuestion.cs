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
		app.MapSrsQuestionEndpoints()
			// 1. Endpoint
			.MapPost("", async (
				[FromBody] CreateBankQuestionRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var command = new CreateBankQuestionCommand(
					request.KnowledgeMeaningId,
					request.QuestionTypeId,
					request.Question,
					request.Answers,
					request.CorrectAnswer,
					request.Explanation,
					request.Difficulty,
					request.IsActive);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetBankQuestionById", id => new { id });
			})
			// 2. Description
			.WithName("CreateBankQuestion")
			.WithSummary("Curator")
			.WithDescription("Creates a new question in the question bank for a specific knowledge meaning.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
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
	string Question,
	IReadOnlyList<QuestionAnswerDto> Answers,
	string CorrectAnswer,
	string? Explanation = null,
	string Difficulty = "Medium",
	bool IsActive = true);
