using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Admin.Users.GetUserById;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;
using Rakushu.Domain.Entities.User;

namespace Rakushu.Api.Endpoints.Admin.Users.GetUsers;

internal sealed class GetUsers : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			// 1. Endpoint
			.MapGet("/users", async(
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? searchTerm,
				[FromQuery] Guid? roleId,
				[FromQuery] UserStatus? status,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetUsersQuery(
					pagination.PageNumber,
					pagination.PageSize,
					searchTerm,
					roleId,
					status
					);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("AdminGetUsers")
			.WithDescription("Retrieves a paginated list of users with optional filtering and search.")
			// 3. Authentication & Authorization: already configure in MapAdminEndpoints()
			// 4. Response
			.Produces<PaginatedList<UserDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
