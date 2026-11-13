using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempts;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuizAttempt.GetQuizAttempts;

internal sealed class GetQuizAttempts : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsAttemptEndpoints()
			.MapGet("/", async (
				ISender sender,
				[FromQuery] string? status = null,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				CancellationToken cancellationToken = default) =>
			{
				int effectivePageNumber = pageNumber > 0 ? pageNumber : 1;
				int effectivePageSize = pageSize > 0 ? pageSize : 10;

				var query = new GetQuizAttemptsQuery(status, effectivePageNumber, effectivePageSize);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetQuizAttempts")
			.WithSummary("Learner")
			.WithDescription("Retrieves a paginated list of quiz attempts with optional status filter.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<PaginatedList<QuizAttemptSummaryDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
