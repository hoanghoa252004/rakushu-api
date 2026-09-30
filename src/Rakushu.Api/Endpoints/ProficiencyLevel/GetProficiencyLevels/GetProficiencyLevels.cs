using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyLevel.GetProficiencyLevels;

namespace Rakushu.Api.Endpoints.ProficiencyLevel.GetProficiencyLevels;

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
