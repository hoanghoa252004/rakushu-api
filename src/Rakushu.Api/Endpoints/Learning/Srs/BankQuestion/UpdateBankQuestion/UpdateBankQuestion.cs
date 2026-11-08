using System.Text.Json;
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
		app.MapSrsQuestionEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateBankQuestionRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				string? mediaMetadata = request.MediaMetadata switch
				{
					{ ValueKind: JsonValueKind.String } el => el.GetString(),
					{ ValueKind: JsonValueKind.Object } el => el.GetRawText(),
					_ => null
				};

				var command = new UpdateBankQuestionCommand(
					id,
					request.Question,
					request.Answers,
					request.CorrectAnswer,
					request.Explanation,
					request.Difficulty,
					request.MediaType,
					request.MediaUrl,
					mediaMetadata);

				var result = await sender.Send(command, cancellationToken);
				return result.MatchNoContent();
			})
			// 2. Description
			.WithName("UpdateBankQuestion")
			.WithSummary("Curator")
			.WithDescription("Updates the question content, answer options, correct answer, explanation, or difficulty level of a bank question.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record UpdateBankQuestionRequest(
	string Question,
	IReadOnlyList<QuestionAnswerDto>? Answers,
	string CorrectAnswer,
	string? Explanation = null,
	string Difficulty = "Medium",
	string? MediaType = null,
	string? MediaUrl = null,
	JsonElement? MediaMetadata = null);