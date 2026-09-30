using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ContentCategory.GetContentCategoryById;

namespace Rakushu.Api.Endpoints.ContentCategory.GetContentCategoryById;

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
