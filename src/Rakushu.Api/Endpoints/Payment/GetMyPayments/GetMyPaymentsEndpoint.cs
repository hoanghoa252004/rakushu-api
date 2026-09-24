using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Payment.GetMyPayments;

namespace Rakushu.Api.Endpoints.Payment.GetMyPayments;

internal sealed class GetMyPaymentsEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			.MapGet("/my-history", async (
				[AsParameters] PaginationRequest pagination,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var query = new GetMyPaymentsQuery(pagination.PageNumber, pagination.PageSize);
				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			.WithName("GetMyPayments")
			.WithDescription("Retrieves the authenticated user's payment history.")
			.RequireAuthorization()
			.Produces<PaginatedList<PaymentSummaryDto>>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
