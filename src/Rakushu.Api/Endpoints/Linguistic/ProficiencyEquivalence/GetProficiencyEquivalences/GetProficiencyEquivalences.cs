using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.GetProficiencyEquivalences;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyEquivalence.GetProficiencyEquivalences;

internal sealed class GetProficiencyEquivalences : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyEquivalenceEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyEquivalencesQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyEquivalences");
	}
}
