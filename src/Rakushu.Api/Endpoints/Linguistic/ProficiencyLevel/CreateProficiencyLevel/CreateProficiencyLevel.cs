using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Linguistic.ProficiencyLevel.CreateProficiencyLevel;

internal sealed class CreateProficiencyLevel : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProficiencyLevelEndpoints()
			// 1. Endpoint
			.MapPost("/", async (CancellationToken cancellationToken) =>
			{
				return Result.Failure(CommonErrors.FeatureNotSupport).MatchOk();
			})
			// 2. Description
			.WithName("CreateProficiencyLevel")
			.WithSummary("Admin")
			.WithDescription("Creates a new proficiency level for linguistic skills.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			// 4. Response
			.Produces<Guid>(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
