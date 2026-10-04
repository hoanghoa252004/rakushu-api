using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

internal sealed class CreateProficiencyLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			.MapPost("/", async ([FromBody] CreateProficiencyLevelCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetProficiencyLevelById", id => new { id });
			})
			.WithName("CreateProficiencyLevel");
	}
}
