using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyFramework.UpdateProficiencyFramework;

namespace Rakushu.Api.Endpoints.ProficiencyFramework.UpdateProficiencyFramework;

internal sealed class UpdateProficiencyFramework : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyFrameworkEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProficiencyFrameworkCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { ProficiencyFrameworkId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateProficiencyFramework");
	}
}
