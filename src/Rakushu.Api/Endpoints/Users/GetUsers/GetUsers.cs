using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Users.GetUserById;
using Rakushu.Application.Usecases.Users.GetUsers;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Users.GetUsers;

internal sealed class GetUsers : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapUserEndpoints()
			// 1. Endpoint
			.MapGet("/users", async(
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? searchTerm,
				[FromQuery] Guid? roleId,
				[FromQuery] string? status,
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
			.WithName("GetUsers")
			.WithDescription("Retrieves a paginated list of users with optional filtering and search.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces<PaginatedList<UserDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
