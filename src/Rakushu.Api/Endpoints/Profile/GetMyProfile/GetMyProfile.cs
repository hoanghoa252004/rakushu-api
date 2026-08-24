using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Profile.GetMyProfile;

namespace Rakushu.Api.Endpoints.Profile.GetMyProfile;

internal sealed class GetMyProfile : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapProfileEndpoints()
			.MapGet("/me", async (ISender sender, CancellationToken cancellationToken) =>
			{
				var query = new GetMyProfileQuery();
				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetMyProfile")
			.WithDescription("Retrieves the currently authenticated user's profile details.")
			.RequireAuthorization()
			.Produces<ProfileResponseDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
