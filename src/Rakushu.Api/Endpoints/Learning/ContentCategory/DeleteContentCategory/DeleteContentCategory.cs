using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.DeleteContentCategory;
using Rakushu.Application.Usecases.Learning.ContentCategory.GetContentCategoryById;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.DeleteContentCategory;

internal sealed class DeleteContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new DeleteContentCategoryCommand(id);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("DeleteContentCategory")
			.WithSummary("Admin")
			.WithDescription("Delete a content Category")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<ContentCategoryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
