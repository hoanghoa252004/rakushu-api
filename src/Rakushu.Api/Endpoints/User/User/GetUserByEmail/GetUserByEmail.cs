using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.User.GetUserByEmail;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.User.User.GetUserByEmail;

internal sealed class GetUserByEmail : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUserEndpoints()
			.MapGet("/by-email", async (
				[FromQuery] string email,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var result = await sender.Send(new GetUserByEmailQuery(email), cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetUserByEmail")
			.WithSummary("Learner")
			.WithDescription("Gets an Active user by email: name, email, avatar (presigned read URL) and JLPT level.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.Learner))
			.Produces<UserLookupDto>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
