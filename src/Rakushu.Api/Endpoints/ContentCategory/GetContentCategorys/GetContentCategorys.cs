using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ContentCategory.GetContentCategorys;

namespace Rakushu.Api.Endpoints.ContentCategory.GetContentCategorys;

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
