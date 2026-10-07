using MediatR;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Payment.Ipn.VnPayIpn;

namespace Rakushu.Api.Endpoints.Payment.Ipn.VnPayIpn;

internal sealed class VnPayIpn : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app // 1. Endpoint
			.MapGet("/api/ipn/vnpay", async (
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
			.WithGroupName("payment")
			.WithName("VnPayIpn")
			.WithSummary("VnPay Gateway")
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
