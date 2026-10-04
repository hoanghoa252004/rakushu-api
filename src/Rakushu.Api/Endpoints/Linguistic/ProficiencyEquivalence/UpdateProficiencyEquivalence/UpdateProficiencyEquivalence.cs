using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.ProficiencyEquivalence;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.UpdateProficiencyEquivalence;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyEquivalence.UpdateProficiencyEquivalence;

internal sealed class UpdateProficiencyEquivalence : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyEquivalenceEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProficiencyEquivalenceCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { ProficiencyEquivalenceId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateProficiencyEquivalence");
	}
}
