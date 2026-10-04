using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.User.Profile.GetProfile;
using Rakushu.Application.Usecases.User.User.GetUserById;
namespace Rakushu.Api.Endpoints.User.Profile.GetProfile;

internal sealed class GetProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			// 1. Endpoint
			.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetProfileQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetProfile")
			.WithDescription("Retrieves the currently authenticated user's profile details.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response 
			.Produces<UserDetailDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
