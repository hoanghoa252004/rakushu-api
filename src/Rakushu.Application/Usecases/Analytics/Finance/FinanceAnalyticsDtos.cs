namespace Rakushu.Application.Usecases.Analytics.Finance;

public sealed record FinanceSummaryDto(
	int Year,
	IReadOnlyList<int> Months,
	string Currency,
	decimal UsdToVndRate,
	decimal TotalRevenue,
	decimal TotalExpenses,
	decimal AwsExpenses,
	decimal AzureExpenses,
	decimal NetProfit,
	decimal ProfitMargin,
	int CompletedPayments,
	IReadOnlyList<ServiceExpenseDto> ExpensesByService,
	IReadOnlyList<MonthlyFinanceDto> MonthlyBreakdown);

public sealed record MonthlyFinanceDto(
	int Month,
	decimal Revenue,
	decimal Expenses,
	decimal AwsExpenses,
	decimal AzureExpenses,
	decimal NetProfit,
	decimal ProfitMargin,
	int CompletedPayments,
	IReadOnlyList<ServiceExpenseDto> ExpensesByService);

public sealed record ServiceExpenseDto(
	string Provider,
	string ServiceName,
	decimal Cost,
	decimal OriginalCost,
	string OriginalCurrency);

public sealed record ExternalServiceUsageDto(
	string Service,
	string Provider,
	string BillingServiceName,
	int Year,
	IReadOnlyList<int> Months,
	string Currency,
	decimal UsdToVndRate,
	DateTimeOffset? FirstUsedAt,
	int? DaysInUse,
	decimal TotalCost,
	decimal TotalOriginalCost,
	string OriginalCurrency,
	IReadOnlyList<string> ConfiguredResources,
	IReadOnlyList<string> UsedResources,
	ServiceJobStatisticsDto? Jobs,
	IReadOnlyList<AiModelUsageDto> AiModels,
	IReadOnlyList<UsageTypeDto> UsageTypes,
	IReadOnlyList<MonthlyServiceUsageDto> MonthlyBreakdown);

public sealed record ServiceJobStatisticsDto(
	int TotalJobs,
	int SucceededJobs,
	int FailedJobs,
	double TotalProcessingSeconds,
	double AverageProcessingSeconds);

public sealed record AiModelUsageDto(
	string ModelId,
	int Requests,
	long InputTokens,
	long OutputTokens);

public sealed record UsageTypeDto(
	string UsageType,
	decimal Quantity,
	string? Unit,
	decimal Cost,
	decimal OriginalCost);

public sealed record MonthlyServiceUsageDto(
	int Month,
	decimal Cost,
	decimal OriginalCost,
	ServiceJobStatisticsDto? Jobs,
	IReadOnlyList<AiModelUsageDto> AiModels,
	IReadOnlyList<UsageTypeDto> UsageTypes);
