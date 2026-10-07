using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.ContentCategory;
using Rakushu.Api.Endpoints.Subscription.Plan;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;
using Rakushu.Application.Usecases.Learning.ContentCategory.UpdateContentCategory;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.UpdateContentCategory;

internal sealed class UpdateContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapPut("/{id:guid}", async(
				[FromRoute] Guid id, 
				[FromBody] UpdateContentCategoryRequestDto dto, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var command = new UpdateContentCategoryCommand(
					id,
					dto.Name,
					dto.JapaneseName,
					dto.DisplayOrder,
					dto.ThemeColor,
					dto.IsActive,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("UpdateContentCategory")
			.WithSummary("Admin")
			.WithDescription("Update a content category.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record UpdateContentCategoryRequestDto(
	string Name,
	string JapaneseName,
	int DisplayOrder,
	string ThemeColor,
	bool IsActive,
	string? Description = null
	);