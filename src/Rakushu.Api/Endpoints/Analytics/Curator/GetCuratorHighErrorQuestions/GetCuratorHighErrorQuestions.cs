using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Curator;
using Rakushu.Application.Usecases.Analytics.Curator.GetCuratorHighErrorQuestions;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Curator.GetCuratorHighErrorQuestions;

internal sealed class GetCuratorHighErrorQuestions : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorAnalyticsEndpoints()
			.MapGet("/high-error-questions", async (
				[FromQuery] int? days,
				[FromQuery] string? timeZone,
				[FromQuery] string? level,
				[FromQuery] string? questionTypeCode,
				[FromQuery] string? search,
				[FromQuery] int? minAnswers,
				[FromQuery] string? sortBy,
				[FromQuery] int? page,
				[FromQuery] int? pageSize,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetCuratorHighErrorQuestionsQuery(
					days ?? 30,
					timeZone,
					level,
					questionTypeCode,
					search,
					minAnswers ?? 1,
					sortBy,
					page ?? 1,
					pageSize ?? 20);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetCuratorHighErrorQuestions")
			.WithSummary("Curator, Administrator")
			.WithDescription("Returns paginated bank questions with learner wrong answers filtered by period, JLPT level, question type, minimum attempts, and search query.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator, RoleCodes.SystemAdministrator))
			.Produces<CuratorHighErrorQuestionsResponseDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
