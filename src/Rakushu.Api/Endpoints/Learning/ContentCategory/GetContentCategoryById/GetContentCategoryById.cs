using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.GetContentCategoryById;

internal sealed class GetContentCategoryById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetContentCategoryByIdQuery(id), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetContentCategoryById");
	}
}
