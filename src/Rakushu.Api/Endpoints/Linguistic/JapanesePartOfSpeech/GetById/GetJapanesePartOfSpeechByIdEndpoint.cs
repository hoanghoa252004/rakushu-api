using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech.GetById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.JapanesePartOfSpeech.GetById;

internal sealed class GetJapanesePartOfSpeechById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetJapanesePartOfSpeechByIdQuery(id), cancellationToken);
				return result is null ? Results.NotFound() : Results.Ok(result);
			})
			.WithName("GetJapanesePartOfSpeechById")
			.WithSummary("Admin")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
