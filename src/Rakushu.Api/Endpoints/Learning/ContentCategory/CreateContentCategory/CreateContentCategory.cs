using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Rakushu.Api.Common;
using Rakushu.Api.Endpoints.Learning.ContentCategory;
using Rakushu.Api.Endpoints.Subscription.Plan;
using Rakushu.Api.Endpoints.Subscription.Plan.CreatePlan;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Learning.ContentCategory.CreateContentCategory;
using Rakushu.Application.Usecases.Subscription.Plan.CreatePlan;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Learning.ContentCategory.CreateContentCategory;

internal sealed class CreateContentCategory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapContentCategoryEndpoints()
			// 1. Endpoint
			.MapPost("/", async (
				[FromBody] CreateContentCategoryRequestDto dto,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var command = new CreateContentCategoryCommand(
					dto.Slug,
					dto.Code,
					dto.Name,
					dto.JapaneseName,
					dto.DisplayOrder,
					dto.ThemeColor,
					dto.IsActive,
					dto.Description
				);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetContentCategoryById", contentId => new { id = contentId });
			})
			// 2. Description
			.WithName("CreateContentCategory")
			.WithSummary("Admin")
			.WithDescription("Creates a content category.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal record CreateContentCategoryRequestDto(
	string Slug,
	string Code,
	string Name,
	string JapaneseName,
	int DisplayOrder,
	string ThemeColor,
	bool IsActive,
	string? Description = null
	);
	