using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.Webhook;
using System.Text.Json;

namespace Rakushu.Api.Endpoints.Payment.SepayWebhook;

internal sealed class SepayWebhookEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPaymentEndpoints()
			.MapPost("/sepay/webhook", async (
				HttpRequest request,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				request.EnableBuffering();
				using var reader = new StreamReader(request.Body, leaveOpen: true);
				var rawBody = await reader.ReadToEndAsync(cancellationToken);
				request.Body.Position = 0;

				SepayWebhookPayload? payload;
				try
				{
					payload = JsonSerializer.Deserialize<SepayWebhookPayload>(rawBody, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});
				}
				catch
				{
					return Results.BadRequest(new { success = false, message = "Invalid JSON payload." });
				}

				if (payload == null)
				{
					return Results.BadRequest(new { success = false, message = "Payload cannot be null." });
				}

				string? signatureHeader = null;
				if (request.Headers.TryGetValue("X-SePay-Signature", out var sig))
				{
					signatureHeader = sig.ToString();
				}
				else if (request.Headers.TryGetValue("X-Signature", out var sig2))
				{
					signatureHeader = sig2.ToString();
				}

				string? timestampHeader = null;
				if (request.Headers.TryGetValue("X-SePay-Timestamp", out var ts))
				{
					timestampHeader = ts.ToString();
				}
				else if (request.Headers.TryGetValue("X-Timestamp", out var ts2))
				{
					timestampHeader = ts2.ToString();
				}

				var command = new ProcessSepayWebhookCommand(payload, rawBody, signatureHeader, timestampHeader);
				var result = await sender.Send(command, cancellationToken);

				if (result.IsFailure)
				{
					return result.Problem();
				}

				return Results.Ok(new { success = true });
			})
			.WithName("SepayWebhook")
			.WithDescription("Processes incoming webhook notifications from SePay for bank transfers.")
			.AllowAnonymous()
			.Produces(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
