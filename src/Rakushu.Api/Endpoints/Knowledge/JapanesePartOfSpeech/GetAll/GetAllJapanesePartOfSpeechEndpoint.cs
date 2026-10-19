using MediatR;
using Rakushu.Api.Common;
using Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.GetAll;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.JapanesePartOfSpeech.GetAll;

internal sealed class GetAllJapanesePartOfSpeech : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapJapanesePartOfSpeechEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetAllJapanesePartOfSpeechQuery(), cancellationToken);
				return Results.Ok(result);
			})
			.WithName("GetAllJapanesePartOfSpeech")
			.WithSummary("Curator")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
			// 4. Response
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
