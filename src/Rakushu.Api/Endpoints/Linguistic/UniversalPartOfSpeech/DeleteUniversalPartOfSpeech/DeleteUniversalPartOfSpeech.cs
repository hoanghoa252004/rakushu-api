using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.DeleteUniversalPartOfSpeech;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.DeleteUniversalPartOfSpeech;

internal sealed class DeleteUniversalPartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapDelete("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteUniversalPartOfSpeechCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteUniversalPartOfSpeech")
			.WithSummary("Admin")
			.WithDescription("Deletes an existing universal part of speech.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
