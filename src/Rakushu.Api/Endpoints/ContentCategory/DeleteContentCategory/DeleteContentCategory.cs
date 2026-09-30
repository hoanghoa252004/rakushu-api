using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.ContentCategory.DeleteContentCategory;

namespace Rakushu.Api.Endpoints.ContentCategory.DeleteContentCategory;

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
