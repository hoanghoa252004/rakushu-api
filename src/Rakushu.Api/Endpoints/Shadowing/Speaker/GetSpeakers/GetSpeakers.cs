using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Shadowing.Speaker;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.Speakers;
using Rakushu.Application.Usecases.Shadowing.Speakers.GetSpeakers;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Shadowing.Speaker.GetSpeakers;

internal sealed class GetSpeakers : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSpeakerEndpoints()
			.MapGet("/", async (
				ISender sender,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10,
				[FromQuery] string? searchTerm = null,
				CancellationToken cancellationToken = default) =>
			{
				var result = await sender.Send(new GetSpeakersQuery(pageNumber, pageSize, searchTerm), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSpeakers")
			.WithSummary("Learner, Curator")
			.WithDescription("Gets speakers with paging. AvatarUrl is a presigned read URL when the speaker has an avatar.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
			.Produces<PaginatedList<SpeakerDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
