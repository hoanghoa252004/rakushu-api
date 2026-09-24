using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Abstractions.Persistence;
using Rakushu.Application.Usecases.Payment.GetPaymentById;

namespace Rakushu.Api.Endpoints.Payment.GetPaymentById;

internal sealed class GetPaymentById : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			// 1. Endpoint
			.MapGet("/{id:guid}", async (
				[FromRoute] Guid id,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				var query = new GetPaymentByIdQuery(id);

				var result = await sender.Send(query, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithName("GetPaymentById")
			.WithDescription("Retrieves payment details by ID. Learners can only access their own payments.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces<PaymentDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
