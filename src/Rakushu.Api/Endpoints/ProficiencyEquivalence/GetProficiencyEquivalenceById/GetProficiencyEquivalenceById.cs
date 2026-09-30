using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyEquivalence.GetProficiencyEquivalenceById;

namespace Rakushu.Api.Endpoints.ProficiencyEquivalence.GetProficiencyEquivalenceById;

internal sealed class GetProficiencyEquivalenceById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyEquivalenceEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyEquivalenceByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyEquivalenceById");
	}
}
