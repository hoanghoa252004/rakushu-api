using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Transaction.CreateTransaction;
using Rakushu.Api.Endpoints.Transaction;

namespace Rakushu.Api.Endpoints.Transaction.CreateTransaction;

internal sealed class CreateTransaction : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapTransactionEndpoints()
			// 1. Endpoint
			.MapPost("/{paymentId:guid}/transactions", async (
				[FromRoute] Guid paymentId,
				[FromBody] CreateTransactionRequestDto dto,
				ISender sender,
				HttpContext context,
				CancellationToken cancellationToken
				) =>
			{

				var ipAddress = context.Connection.RemoteIpAddress?.ToString();

				var command = new CreateTransactionCommand(paymentId, dto.Provider, ipAddress);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchCreated("GetTransactionsByPaymentId", transactionId => new { transactionId });
			})
			// 2. Description
			.WithName("CreateTransaction")
			.WithDescription("Creates a new transaction for the specified payment.")
			// 3. Authentication & Authorization
			.RequireAuthorization()
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}

internal sealed record CreateTransactionRequestDto(string Provider);
