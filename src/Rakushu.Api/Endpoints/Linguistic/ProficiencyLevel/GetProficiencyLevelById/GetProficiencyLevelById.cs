using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.GetProficiencyLevelById;

internal sealed class GetProficiencyLevelById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyLevelByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyLevelById");
	}
}
