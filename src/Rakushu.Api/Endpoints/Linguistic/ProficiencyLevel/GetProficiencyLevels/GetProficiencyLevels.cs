using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.GetProficiencyLevels;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.GetProficiencyLevels;

internal sealed class GetProficiencyLevels : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyLevelsQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyLevels");
	}
}
