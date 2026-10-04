using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.CreateProficiencyFramework;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework.CreateProficiencyFramework;

internal sealed class CreateProficiencyFramework : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyFrameworkEndpoints()
			.MapPost("/", async ([FromBody] CreateProficiencyFrameworkCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetProficiencyFrameworkById", id => new { id });
			})
			.WithName("CreateProficiencyFramework");
	}
}
