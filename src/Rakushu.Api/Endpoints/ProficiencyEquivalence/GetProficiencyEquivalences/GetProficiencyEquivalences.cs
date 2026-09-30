using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalences;

namespace Rakushu.Api.Endpoints.ProficiencyEquivalence.GetProficiencyEquivalences;

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
