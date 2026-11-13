using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemSchedule;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Srs.SavedItem.GetSavedItemSchedule;

internal sealed class GetSavedItemSchedule : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSrsScheduleEndpoints()
			.MapGet("/{savedItemId:guid}", async (
				Guid savedItemId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetSavedItemScheduleQuery(savedItemId);
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetSavedItemSchedule")
			.WithSummary("Learner")
			.WithDescription("Retrieves current SM-2 scheduling parameters (interval, repetitions, lapses, easiness factor) and complete review audit log history for a specific saved item.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<SavedItemScheduleDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
