using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Linguistic.ProficiencyFramework.DeleteProficiencyFramework;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyFramework.DeleteProficiencyFramework;

internal sealed class DeleteProficiencyFramework : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyFrameworkEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteProficiencyFrameworkCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteProficiencyFramework");
	}
}
