using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Payment.GetPayments;
using GetPaymentsQuery = Rakushu.Application.Usecases.Payment.GetPayments.GetPaymentsQuery;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.GetPayments;

internal sealed class GetPayments : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			// 1. Endpoint
			.MapGet("/", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] Guid? userId,
				[FromQuery] Guid? planId,
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetPaymentsQuery(
					pagination.PageNumber,
					pagination.PageSize,
					userId,
					planId,
					status);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetPayments")
			.WithDescription("Retrieves a paginated list of all payments with optional filtering.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			// 4. Response
			.Produces<PaginatedList<PaymentDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
