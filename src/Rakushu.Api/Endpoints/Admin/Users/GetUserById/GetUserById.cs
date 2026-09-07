using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;

namespace Rakushu.Api.Endpoints.Admin.Users.GetUserById;

internal sealed class GetUserById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			// 1. Endpoint
			.MapGet("/users/{id:guid}", async (
				[FromRoute] Guid id, 
				ISender sender, 
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetUserByIdQuery(id);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("AdminGetUserById")
			.WithDescription("Retrieves detailed user profile and account details by user ID.")
			// 3. Authentication & Authorization: already configure in MapAdminEndpoints()
			// 4. Response
			.Produces<UserDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
