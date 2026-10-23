using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Material.Transcript;
using Rakushu.Application.Usecases.Material.TranscriptSegment.GetSegmentById;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Material.Transcript.GetSegmentById;

internal sealed class GetTranscriptSegmentById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTranscriptEndpoints()
			.MapGet("/segments/{segmentId:guid}", async (
			[FromRoute] Guid videoId,
			[FromRoute] Guid segmentId,
			ISender sender,
			CancellationToken cancellationToken) =>
		{
			var query = new GetTranscriptSegmentByIdQuery(videoId, segmentId);

			var result = await sender.Send(query, cancellationToken);

			return result.MatchOk();
		})
		.WithName("GetTranscriptSegmentById")
		.WithSummary("Curator")
		.WithDescription("Retrieves a specific transcript segment with bunsetsus and tokens detail.")
		.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
		.Produces<TranscriptSegmentDetailDto>(StatusCodes.Status200OK)
		.ProducesProblem(StatusCodes.Status404NotFound)
		.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
