using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.DeleteContentCategory;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.DeleteContentCategory;

internal sealed class DeleteContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new DeleteContentCategoryCommand(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("DeleteContentCategory");
	}
}
