using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.UpdateUniversalPartOfSpeech;

internal sealed class UpdateUniversalPartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapPut("/{id:guid}", async (
				[FromRoute] Guid id,
				[FromBody] UpdateUniversalPartOfSpeechCommand command,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { UniversalPartOfSpeechId = id }, cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("UpdateUniversalPartOfSpeech")
			.WithSummary("Admin")
			.WithDescription("Updates an existing universal part of speech.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
