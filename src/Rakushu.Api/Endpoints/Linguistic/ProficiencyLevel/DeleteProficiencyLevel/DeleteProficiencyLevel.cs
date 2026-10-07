using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.DeleteProficiencyLevel;

internal sealed class DeleteProficiencyLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			// 1. Endpoint
			.MapDelete("/{id:guid}", async ([FromRoute] Guid id, CancellationToken cancellationToken) =>
			{
				return Result.Failure(CommonErrors.FeatureNotSupport).MatchOk();
			})
			// 2. Description
			.WithName("DeleteProficiencyLevel")
			.WithSummary("Admin")
			.WithDescription("Deletes an existing proficiency level for linguistic skills.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

