using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategorys;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.GetContentCategorys;

internal sealed class GetContentCategorys : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(new GetContentCategorysQuery(), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetContentCategorys");
	}
}
