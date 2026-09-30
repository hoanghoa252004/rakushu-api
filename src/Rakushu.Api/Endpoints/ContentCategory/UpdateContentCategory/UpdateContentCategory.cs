using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ContentCategory.UpdateContentCategory;

namespace Rakushu.Api.Endpoints.ContentCategory.UpdateContentCategory;

internal sealed class UpdateContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateContentCategoryCommand command, ISender sender, CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(command with { ContentCategoryId = id }, cancellationToken);
				return result.MatchOk();
			})
			.WithName("UpdateContentCategory");
	}
}
