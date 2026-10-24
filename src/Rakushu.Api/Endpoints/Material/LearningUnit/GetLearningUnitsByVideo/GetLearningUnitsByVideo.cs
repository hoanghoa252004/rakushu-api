using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.LearningUnit;
using Rakushu.Application.Usecases.Material.LearningUnit.GetLearningUnitsByVideo;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.LearningUnit.GetLearningUnitsByVideo;

internal sealed class GetLearningUnitsByVideo : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/api/videos/{videoId:guid}/learning-units", async (
				[FromRoute] Guid videoId,
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? type = null,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(
					new GetVideoLearningUnitsQuery(videoId, pageNumber, pageSize, type), cancellationToken);
				return result.MatchOk();
			})
			.WithGroupName("material")
			.WithTags("Learning Units")
			.WithName("GetVideoLearningUnits")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets learning units of a video with paging. Filter: type (Word, Collocation, Grammar, Terminology, ProperNoun).")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<LearningUnitSummaryDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}