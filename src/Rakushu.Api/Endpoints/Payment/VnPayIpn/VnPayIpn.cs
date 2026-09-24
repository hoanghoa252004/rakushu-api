using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.VnPayIpn;
using Rakushu.Domain.Entities.Role;

namespace Rakushu.Api.Endpoints.Payment.VnPayIpn;

internal sealed class VnPayIpn : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app // 1. Endpoint
			.MapGet("/vnpay-ipn", async (
				HttpContext httpContext,
				ISender sender,
				CancellationToken cancellationToken
				) =>
			{
				IReadOnlyDictionary<string, string> parameters =
					httpContext.Request.Query.ToDictionary(
						x => x.Key,
						x => x.Value.ToString()
					);

				var command = new VnPayIpnCommand(parameters);

				var result = await sender.Send(command, cancellationToken);

				return result.MatchOk();
			})
			// 2. Description
			.WithTags("IPN")
			.WithGroupName("subscription")
			.WithName("VnPayIpn")
			.WithDescription("Handles the VnPay IPN callback of VNPAY.")
			// 3. Authentication & Authorization
			.AllowAnonymous()
			// 4. Response
			.Produces(StatusCodes.Status201Created)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status404NotFound)
			.ProducesProblem(StatusCodes.Status409Conflict)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
