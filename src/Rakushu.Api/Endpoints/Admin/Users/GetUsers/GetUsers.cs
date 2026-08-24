using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Admin.Users.GetUsers;

namespace Rakushu.Api.Endpoints.Admin.Users.GetUsers;

internal sealed class GetUsers : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapAdminEndpoints()
			.MapGet("/users", async (
				[FromQuery] int pageNumber,
				[FromQuery] int pageSize,
				[FromQuery] string? searchTerm,
				[FromQuery] Guid? roleId,
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetUsersListQuery(
					pageNumber <= 0 ? 1 : pageNumber,
					pageSize <= 0 ? 10 : pageSize,
					searchTerm,
					roleId,
					status);

				var result = await sender.Send(query, cancellationToken);
				return result.MatchOk();
			})
			.WithName("AdminGetUsers")
			.WithDescription("Retrieves a paginated list of users with optional filtering and search.")
			.Produces<PaginatedList<UserSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
