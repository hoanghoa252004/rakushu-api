using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rakushu.Api.Common;
using Rakushu.Api.Extensions;
using Rakushu.Application.Usecases.Analytics.Finance;
using Rakushu.Application.Usecases.Analytics.Finance.GetFinanceSummary;
using Rakushu.Domain.Entities.Identity.Role;

namespace Rakushu.Api.Endpoints.Analytics.Finance;

internal sealed class GetFinanceSummary : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapFinanceAnalyticsEndpoints()
			.MapGet("/summary", async (
				[FromQuery] int year,
				[FromQuery] int[]? months,
				[FromQuery] bool? sortByProfitAscending,
				ISender sender,
				CancellationToken cancellationToken) =>
			{
				var result = await sender.Send(
					new GetFinanceSummaryQuery(year, months, sortByProfitAscending ?? false),
					cancellationToken);
				return result.MatchOk();
			})
			.WithName("GetFinanceSummary")
			.WithSummary("Administrator")
			.WithDescription("Returns total revenue (completed payments), total expenses (AWS Cost Explorer + Azure Cost Management), net profit and profit margin in VND for the selected months of a year (e.g. ?year=2026&months=4&months=5&months=6). Omit months for all 12 months. sortByProfitAscending=true sorts the monthly breakdown by net profit ascending.")
			.RequireAuthorization(policy => policy.RequireRole(RoleCodes.SystemAdministrator))
			.Produces<FinanceSummaryDto>(StatusCodes.Status200OK)
			.ProducesProblem(StatusCodes.Status400BadRequest)
			.ProducesProblem(StatusCodes.Status401Unauthorized)
			.ProducesProblem(StatusCodes.Status403Forbidden)
			.ProducesProblem(StatusCodes.Status500InternalServerError);
	}
}
