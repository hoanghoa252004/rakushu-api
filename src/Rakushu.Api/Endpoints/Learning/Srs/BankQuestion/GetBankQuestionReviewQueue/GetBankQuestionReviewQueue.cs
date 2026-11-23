using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion;
using Rakushu.Application.Usecases.Learning.Srs.BankQuestion.GetBankQuestionReviewQueue;
using Rakushu.Domain.Entities.Identity.Role;
using Rakushu.Domain.Entities.Srs.BankQuestion;

namespace Rakushu.Api.Endpoints.Learning.Srs.BankQuestion.GetBankQuestionReviewQueue;

internal sealed class GetBankQuestionReviewQueue : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuestionEndpoints()
			// 1. Endpoint
			.MapGet("/review-queue", async (
				[FromQuery] QuestionStatus? status,
				[FromQuery] string? level,
				[FromQuery] string? questionTypeCode,
				[FromQuery] string? mediaType,
				[FromQuery] string? search,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetBankQuestionReviewQueueQuery(
					status ?? QuestionStatus.Draft,
					level,
					questionTypeCode,
					mediaType,
					search,
					page ?? 1,
					pageSize ?? 20);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetBankQuestionReviewQueue")
			.WithSummary("Curator, Administrator")
			.WithDescription("Retrieves a paginated review queue of bank questions (defaulting to Draft) with filters by JLPT level, question type code, media type, and search keyword.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<BankQuestionReviewQueueResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
