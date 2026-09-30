using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyEquivalence.DeleteProficiencyEquivalence;

namespace Rakushu.Api.Endpoints.ProficiencyEquivalence.DeleteProficiencyEquivalence;

internal sealed class DeleteProficiencyEquivalence : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyEquivalenceEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteProficiencyEquivalenceCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteProficiencyEquivalence");
	}
}
