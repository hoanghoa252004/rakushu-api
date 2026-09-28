using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Curator.Oov.IngestOovCandidates;
using System.Collections.Generic;
using System.Threading;

namespace Rakushu.Api.Endpoints.Curator.IngestOovCandidates;

internal sealed class IngestOovCandidates : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapCuratorEndpoints()
			.MapPost("/ingest", async (
				[FromBody] List<IngestOovItem> items,
				ISender sender,
				CancellationToken cancellationToken
			) =>
			{
				var command = new IngestOovCandidatesCommand(items);
				var result = await sender.Send(command, cancellationToken);
				return result.MatchOk();
			})
			.WithName("IngestOovCandidates")
			.WithDescription("Ingests novel OOV candidates discovered by AI video processing pipeline.")
			.Produces<int>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
