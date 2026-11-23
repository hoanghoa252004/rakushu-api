using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs.QuestionType;
using Rakushu.Application.Usecases.Learning.Srs.QuestionType.GetQuestionTypes;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.QuestionType.GetQuestionTypes;

internal sealed class GetQuestionTypes : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsQuestionTypeEndpoints()
			.MapGet("", async (
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetQuestionTypesQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetQuestionTypes")
			.WithSummary("Curator")
			.WithDescription("Retrieves a list of all active SRS question types for question curation.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			.Produces<IReadOnlyList<QuestionTypeDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
