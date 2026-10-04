using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.GetProficiencyFrameworkById;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework.GetProficiencyFrameworkById;

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
