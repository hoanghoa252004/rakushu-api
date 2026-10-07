using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.UniversalPartOfSpeech.GetUniversalPartOfSpeechById;

internal sealed class GetUniversalPartOfSpeechById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUniversalPartOfSpeechEndpoints()
			.MapGet("/{id:guid}", async ([FromRoute] Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetUniversalPartOfSpeechByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetUniversalPartOfSpeechById")
			.WithSummary("Admin")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
