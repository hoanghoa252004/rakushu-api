using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Series.CreateSeries;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Series.CreateSeries;

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
				request.JapaneseName,
				request.ThumbnailKey,
				request.Description,
				request.DisplayOrder,
				request.IsActive);

			var result = await sender.Send(command, cancellationToken);

			return result.MatchCreated("GetSeriesById", seriesId => new { id = seriesId });
		})
		.WithName("CreateSeries")
		.WithSummary("Learner & Curator")
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
	string JapaneseName,
	string? ThumbnailKey,
	string? Description,
	int DisplayOrder,
	bool IsActive);
