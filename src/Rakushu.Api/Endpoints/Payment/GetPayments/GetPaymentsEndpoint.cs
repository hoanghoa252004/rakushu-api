using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.GetPayments;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.GetPayments;

internal sealed class GetPaymentsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			.MapGet(string.Empty, async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] string? status,
				[FromQuery] string? orderCode,
				[FromQuery] Guid? userId,
				[FromQuery] Guid? planId,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetPaymentsQuery(
					pagination.PageNumber,
					pagination.PageSize,
					status,
					orderCode,
					userId,
					planId);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetPayments")
			.WithDescription("Retrieves a paginated list of all payments with optional filtering (Admin only).")
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.SystemAdministrator.ToString()))
			.Produces<PaginatedList<PaymentAdminDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
