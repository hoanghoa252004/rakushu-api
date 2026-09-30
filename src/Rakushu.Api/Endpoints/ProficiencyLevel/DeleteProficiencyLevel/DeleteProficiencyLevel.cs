using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyLevel.DeleteProficiencyLevel;

namespace Rakushu.Api.Endpoints.ProficiencyLevel.DeleteProficiencyLevel;

internal sealed class DeleteProficiencyLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteProficiencyLevelCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteProficiencyLevel");
	}
}
