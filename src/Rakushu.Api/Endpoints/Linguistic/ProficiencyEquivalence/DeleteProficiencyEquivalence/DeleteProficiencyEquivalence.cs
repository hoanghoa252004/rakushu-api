using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.DeleteProficiencyEquivalence;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyEquivalence.DeleteProficiencyEquivalence;

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
