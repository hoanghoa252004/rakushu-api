using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestions;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.GetBankQuestions;

internal sealed class GetBankQuestions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuestionEndpoints()
			// 1. Endpoint
			.MapGet("", async (
				[FromQuery] Guid? meaningId,
				[FromQuery] Guid? questionTypeId,
				[FromQuery] bool? isActive,
				[FromQuery] QuestionStatus? status,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetBankQuestionsQuery(
					meaningId,
					questionTypeId,
					isActive,
					status,
					page <= 0 ? 1 : page ?? 1,
					pageSize <= 0 ? 20 : pageSize ?? 20);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetBankQuestions")
			.WithSummary("Curator")
			.WithDescription("Retrieves a paginated list of bank questions with optional filters by knowledge meaning, question type, review status (Draft, Published), and active status.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<IReadOnlyList<BankQuestionSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
