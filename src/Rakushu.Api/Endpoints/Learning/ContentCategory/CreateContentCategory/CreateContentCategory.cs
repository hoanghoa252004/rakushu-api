using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.ContentCategory;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.CreateContentCategory;

internal sealed class CreateContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapPost("/", async ([FromBody] CreateContentCategoryCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command, cancellationToken);
				return result.MatchCreated("GetContentCategoryById", id => new { id });
			})
			.WithName("CreateContentCategory");
	}
}
