using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Profile.GetProfile;

namespace Rakushu.Api.Endpoints.Profile.GetProfile;

internal sealed class GetProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			// 1. Endpoint
			.MapGet("/me", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetProfileQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetMyProfile")
			.WithDescription("Retrieves the currently authenticated user's profile details.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response 
			.Produces<UserDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
