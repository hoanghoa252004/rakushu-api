using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.GetMyPaymentHistory;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.GetMyPaymentHistory;

internal sealed class GetMyPaymentHistory : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			// 1. Endpoint
			.MapGet("/my-history", async (
				[AsParameters] PaginationRequest pagination,
				[FromQuery] Guid? planId,
				[FromQuery] string? status,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetMyPaymentHistoryQuery(
					pagination.PageNumber,
					pagination.PageSize,
					planId,
					status
					);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetMyPaymentHistory")
			.WithDescription("Retrieves the payment history of the current learner with all transactions.")
			// 3. Authentication & Authorization
			.RequireAuthorization(policy => policy.RequireRole(DefaultSystemRoles.Learner.ToString()))
			// 4. Response
			.Produces<PaginatedList<PaymentDto>>(StatusCodes.Status200OK)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
