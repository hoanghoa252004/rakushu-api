using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestions;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.GetBankQuestions;

internal sealed class GetBankQuestions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsBankQuestionEndpoints()
			.MapGet("", async (
				[FromQuery] Guid? meaningId,
				[FromQuery] Guid? questionTypeId,
				[FromQuery] string? difficulty,
				[FromQuery] bool? isActive,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetBankQuestionsQuery(
					meaningId,
					questionTypeId,
					difficulty,
					isActive,
					page <= 0 ? 1 : page ?? 1,
					pageSize <= 0 ? 20 : pageSize ?? 20);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetBankQuestions")
			.WithSummary("List bank questions with filters")
			.WithDescription("Allows linguistic curators and admins to query bank questions by knowledge meaning, question type, difficulty, and active status.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<IReadOnlyList<BankQuestionSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
