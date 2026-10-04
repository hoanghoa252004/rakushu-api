using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.GetProficiencyFrameworks;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework.GetProficiencyFrameworks;

internal sealed class GetProficiencyFrameworks : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyFrameworkEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetProficiencyFrameworksQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetProficiencyFrameworks");
	}
}
