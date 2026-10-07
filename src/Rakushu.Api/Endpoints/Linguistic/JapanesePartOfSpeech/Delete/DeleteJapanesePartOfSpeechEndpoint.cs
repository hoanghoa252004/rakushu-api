using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.Delete;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.Delete;

internal sealed class DeleteJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapDelete("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteJapanesePartOfSpeechCommand(id), cancellationToken);
				return result.MatchNoContent();
			})
			.WithName("DeleteJapanesePartOfSpeech")
			.WithSummary("Admin")
			.WithDescription("Deletes an existing Japanese part of speech.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status204NoContent)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
