using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.ContentCategory.CreateContentCategory;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.ContentCategory.CreateContentCategory;

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
			.WithSummary("Curator")
			.WithDescription("Creates a content category.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
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
	