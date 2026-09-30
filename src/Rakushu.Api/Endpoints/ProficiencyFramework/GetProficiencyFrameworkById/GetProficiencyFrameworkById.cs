using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ProficiencyFramework.GetProficiencyFrameworkById;

namespace Rakushu.Api.Endpoints.ProficiencyFramework.GetProficiencyFrameworkById;

internal sealed class GetProficiencyFrameworkById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyFrameworkEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyFrameworkByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyFrameworkById");
	}
}
