using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Material.Series;
using Rakushu.Application.Usecases.Material.Series.GetMySeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Series.GetMySeries;

internal sealed class GetMySeries : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapGet("/me", async (
			ISender sender,
			CancellationToken cancellationToken = default) =>
			{
				var query = new GetMySeriesQuery();

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
		.WithName("GetMySeries")
		.WithSummary("Learner")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
		.Produces<IReadOnlyCollection<SeriesDto>>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
