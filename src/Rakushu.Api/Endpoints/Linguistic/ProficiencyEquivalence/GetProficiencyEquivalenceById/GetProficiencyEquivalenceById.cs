using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyEquivalence.GetProficiencyEquivalenceById;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyEquivalence.GetProficiencyEquivalenceById;

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
