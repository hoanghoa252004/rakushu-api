using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.Series.CreateSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Learning.Series.CreateSeries;

internal sealed class CreateSeries : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapSeriesEndpoints()
			.MapPost("/", async (
			CreateSeriesRequest request,
			ISender sender,
			CancellationToken cancellationToken) =>
		{

			var command = new CreateSeriesCommand(
				request.Slug,
				request.Name,
				request.Description,
				request.DisplayOrder);

			var result = await sender.Send(command, cancellationToken);


			return result.MatchCreated("GetSeriesById", seriesId => new { id = seriesId });
		})
		.WithName("CreateSeries")
		.WithSummary("Create a new series")
		.WithDescription("Creates a new series. Learners create learner-sourced series, curators create curated series.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner, RoleCodes.LinguisticCurator))
		.Produces<Guid>(StatusCodes.Status201Created)
		.ProducesProblem(StatusCodes.Status400BadRequest)
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

public sealed record CreateSeriesRequest(
	string Slug,
	string Name,
	string? Description,
	int DisplayOrder);

public sealed record CreateSeriesResponse(Guid Id);
