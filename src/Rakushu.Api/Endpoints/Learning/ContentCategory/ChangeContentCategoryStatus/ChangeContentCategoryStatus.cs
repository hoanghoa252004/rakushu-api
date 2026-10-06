using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.ContentCategory;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.ChangeContentCategoryStatus;
using Rakushu.Domain.Entities.ContentCategory;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.ChangeContentCategoryStatus;

internal sealed class ChangeContentCategoryStatus : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			.MapPatch("/{id}/status", async (Guid id, [FromBody] ChangeStatusRequest request, ISender sender, CancellationToken cancellationToken) =>
			{
				var command = new ChangeContentCategoryStatusCommand(id, request.Status);
				var result = await sender.Send(command, cancellationToken);
				return result.IsSuccess 
					? Results.NoContent() 
					: Results.BadRequest(result.Error);
			})
			.WithName("ChangeContentCategoryStatus")
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status400BadRequest)
			.Produces(StatusCodes.Status404NotFound);
	}

	public sealed record ChangeStatusRequest(ContentCategoryStatus Status);
}
