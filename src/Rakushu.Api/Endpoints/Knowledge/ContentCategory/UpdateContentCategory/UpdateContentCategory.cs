using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Knowledge.ContentCategory.UpdateContentCategory;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Knowledge.ContentCategory.UpdateContentCategory;

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
			.WithSummary("Curator")
			.WithDescription("Update a content category.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.LinguisticCurator))
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