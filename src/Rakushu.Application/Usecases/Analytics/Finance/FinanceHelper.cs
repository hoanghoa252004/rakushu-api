using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;

namespace Rakushu.Application.Usecases.Analytics.Finance;

internal static class FinanceHelper
{
	public const string BaseCurrency = "VND";

	public static IReadOnlyList<int> NormalizeMonths(IReadOnlyCollection<int>? months)
		=> months is null || months.Count == 0
			? Enumerable.Range(1, 12).ToList()
			: months.Distinct().OrderBy(m => m).ToList();

	public static Task<decimal> GetUsdToVndRateAsync(ISystemConfigurationProvider config, CancellationToken cancellationToken)
		=> config.GetDecimalAsync(SystemConfigurationKey.FinanceSettings_UsdToVndRate, cancellationToken);

	public static decimal ToVnd(decimal amount, string currency, decimal usdToVndRate)
		=> string.Equals(currency, BaseCurrency, StringComparison.OrdinalIgnoreCase)
			? amount
			: Math.Round(amount * usdToVndRate, 0, MidpointRounding.AwayFromZero);

	public static decimal ProfitMargin(decimal revenue, decimal netProfit)
		=> revenue == 0 ? 0 : Math.Round(netProfit / revenue * 100, 2, MidpointRounding.AwayFromZero);
}
