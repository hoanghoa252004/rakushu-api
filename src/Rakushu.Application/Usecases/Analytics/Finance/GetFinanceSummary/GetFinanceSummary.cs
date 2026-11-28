using FluentValidation;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Billing;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Analytics.Finance.GetFinanceSummary;

/// <param name="Months">Months of the year to include (1..12). Empty = all 12 months.</param>
/// <param name="SortByProfitAscending">Sort the monthly breakdown by net profit ascending (default: by month).</param>
public sealed record GetFinanceSummaryQuery(
	int Year,
	IReadOnlyCollection<int>? Months = null,
	bool SortByProfitAscending = false) : IRequest<Result<FinanceSummaryDto>>;

internal sealed class GetFinanceSummaryValidator : AbstractValidator<GetFinanceSummaryQuery>
{
	public GetFinanceSummaryValidator()
	{
		RuleFor(x => x.Year)
			.InclusiveBetween(2000, 2100)
			.WithMessage("Year must be between 2000 and 2100.");

		RuleForEach(x => x.Months)
			.InclusiveBetween(1, 12)
			.WithMessage("Month must be between 1 and 12.");
	}
}

internal sealed class GetFinanceSummaryHandler : IRequestHandler<GetFinanceSummaryQuery, Result<FinanceSummaryDto>>
{
	private readonly IFinanceAnalyticsQueryService _queryService;
	private readonly ICloudBillingService _billingService;
	private readonly ISystemConfigurationProvider _config;

	public GetFinanceSummaryHandler(
		IFinanceAnalyticsQueryService queryService,
		ICloudBillingService billingService,
		ISystemConfigurationProvider config)
	{
		_queryService = queryService;
		_billingService = billingService;
		_config = config;
	}

	public async Task<Result<FinanceSummaryDto>> Handle(GetFinanceSummaryQuery request, CancellationToken cancellationToken)
	{
		var months = FinanceHelper.NormalizeMonths(request.Months);
		var rate = await FinanceHelper.GetUsdToVndRateAsync(_config, cancellationToken);

		var revenueRows = await _queryService.GetMonthlyRevenueAsync(request.Year, months, cancellationToken);
		var costRows = await _billingService.GetMonthlyCostsAsync(request.Year, months, cancellationToken);

		var monthly = months.Select(month =>
		{
			var revenue = revenueRows
				.Where(r => r.Month == month)
				.Sum(r => FinanceHelper.ToVnd(r.Amount, r.Currency, rate));
			var payments = revenueRows.Where(r => r.Month == month).Sum(r => r.PaymentCount);

			var services = BuildServiceExpenses(costRows.Where(c => c.Month == month), rate);
			var aws = services.Where(s => s.Provider == nameof(CloudProvider.Aws)).Sum(s => s.Cost);
			var azure = services.Where(s => s.Provider == nameof(CloudProvider.Azure)).Sum(s => s.Cost);
			var expenses = aws + azure;
			var profit = revenue - expenses;

			return new MonthlyFinanceDto(
				month, revenue, expenses, aws, azure, profit,
				FinanceHelper.ProfitMargin(revenue, profit), payments, services);
		}).ToList();

		var ordered = request.SortByProfitAscending
			? monthly.OrderBy(m => m.NetProfit).ThenBy(m => m.Month).ToList()
			: monthly;

		var totalRevenue = monthly.Sum(m => m.Revenue);
		var totalExpenses = monthly.Sum(m => m.Expenses);
		var netProfit = totalRevenue - totalExpenses;

		return Result.Success(new FinanceSummaryDto(
			request.Year,
			months,
			FinanceHelper.BaseCurrency,
			rate,
			totalRevenue,
			totalExpenses,
			monthly.Sum(m => m.AwsExpenses),
			monthly.Sum(m => m.AzureExpenses),
			netProfit,
			FinanceHelper.ProfitMargin(totalRevenue, netProfit),
			monthly.Sum(m => m.CompletedPayments),
			BuildServiceExpenses(costRows, rate),
			ordered));
	}

	private static List<ServiceExpenseDto> BuildServiceExpenses(IEnumerable<CloudServiceMonthlyCost> rows, decimal rate)
		=> rows
			.GroupBy(r => new { r.Provider, r.ServiceName, r.Currency })
			.Select(g =>
			{
				var original = g.Sum(x => x.Cost);
				return new ServiceExpenseDto(
					g.Key.Provider.ToString(),
					g.Key.ServiceName,
					FinanceHelper.ToVnd(original, g.Key.Currency, rate),
					original,
					g.Key.Currency);
			})
			.Where(s => s.OriginalCost != 0)
			.OrderByDescending(s => s.Cost)
			.ToList();
}
