using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyEquivalence.CreateProficiencyEquivalence;

namespace Rakushu.Api.Endpoints.ProficiencyEquivalence.CreateProficiencyEquivalence;

internal sealed class CreateProficiencyEquivalence : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyEquivalenceEndpoints()
			.MapPost("/", async ([FromBody] CreateProficiencyEquivalenceCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetProficiencyEquivalenceById", id => new { id });
			})
			.WithName("CreateProficiencyEquivalence");
	}
}
