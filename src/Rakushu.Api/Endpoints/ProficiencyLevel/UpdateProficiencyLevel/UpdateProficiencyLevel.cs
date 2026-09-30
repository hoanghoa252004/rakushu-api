using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyLevel.UpdateProficiencyLevel;

namespace Rakushu.Api.Endpoints.ProficiencyLevel.UpdateProficiencyLevel;

internal sealed class UpdateProficiencyLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateProficiencyLevelCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { ProficiencyLevelId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateProficiencyLevel");
	}
}
