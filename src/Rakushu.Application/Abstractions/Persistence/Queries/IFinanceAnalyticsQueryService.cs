namespace Rakushu.Application.Abstractions.Persistence.Queries;

public sealed record MonthlyRevenueRow(int Month, string Currency, decimal Amount, int PaymentCount);

public sealed record MonthlyJobUsageRow(int Month, int TotalJobs, int SucceededJobs, int FailedJobs, double ProcessingSeconds);

public sealed record MonthlyModelUsageRow(int Month, string ModelId, int Requests, long InputTokens, long OutputTokens);

public interface IFinanceAnalyticsQueryService
{
	/// <summary>Completed payments grouped by month (UTC, by UpdatedAt) and currency.</summary>
	Task<IReadOnlyList<MonthlyRevenueRow>> GetMonthlyRevenueAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default);

	/// <summary>Processing jobs run on an external service grouped by month (UTC, by StartAt).</summary>
	Task<IReadOnlyList<MonthlyJobUsageRow>> GetMonthlyJobUsageAsync(
		string jobServiceName,
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default);

	Task<DateTimeOffset?> GetFirstJobUsedAtAsync(string jobServiceName, CancellationToken cancellationToken = default);

	/// <summary>AI assistant messages grouped by month (UTC, by CreatedAt) and model.</summary>
	Task<IReadOnlyList<MonthlyModelUsageRow>> GetMonthlyAiModelUsageAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default);

	Task<DateTimeOffset?> GetFirstAiUsedAtAsync(CancellationToken cancellationToken = default);
}
