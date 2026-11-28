using Microsoft.EntityFrameworkCore;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Domain.Entities.Assisstant.Message;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;
using Rakushu.Domain.Entities.Subscriptions.Payment;

namespace Rakushu.Persistence.Queries;

internal sealed class FinanceAnalyticsQueryService : IFinanceAnalyticsQueryService
{
	private readonly RakushuDbContext _context;

	public FinanceAnalyticsQueryService(RakushuDbContext context)
	{
		_context = context;
	}

	public async Task<IReadOnlyList<MonthlyRevenueRow>> GetMonthlyRevenueAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default)
	{
		var (from, to) = GetRange(year, months);
		var fromOffset = new DateTimeOffset(from, TimeSpan.Zero);
		var toOffset = new DateTimeOffset(to, TimeSpan.Zero);

		var payments = await _context.Payments
			.AsNoTracking()
			.Where(p => p.Status == PaymentStatus.Completed && p.UpdatedAt >= fromOffset && p.UpdatedAt < toOffset)
			.Select(p => new { p.UpdatedAt, p.Currency, p.Amount })
			.ToListAsync(cancellationToken);

		return payments
			.Select(p => new { Month = p.UpdatedAt.UtcDateTime.Month, p.Currency, p.Amount })
			.Where(p => months.Contains(p.Month))
			.GroupBy(p => new { p.Month, p.Currency })
			.Select(g => new MonthlyRevenueRow(g.Key.Month, g.Key.Currency.ToString(), g.Sum(x => x.Amount), g.Count()))
			.ToList();
	}

	public async Task<IReadOnlyList<MonthlyJobUsageRow>> GetMonthlyJobUsageAsync(
		string jobServiceName,
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default)
	{
		var (from, to) = GetRange(year, months);

		var jobs = await _context.Set<ProcessingJobLog>()
			.AsNoTracking()
			.Where(j => j.ServiceName == jobServiceName && j.StartAt >= from && j.StartAt < to)
			.Select(j => new { j.StartAt, j.EndAt, j.IsSuccess, j.IsFailed })
			.ToListAsync(cancellationToken);

		return jobs
			.Where(j => months.Contains(j.StartAt.Month))
			.GroupBy(j => j.StartAt.Month)
			.Select(g => new MonthlyJobUsageRow(
				g.Key,
				g.Count(),
				g.Count(j => j.IsSuccess),
				g.Count(j => j.IsFailed),
				g.Where(j => j.EndAt.HasValue).Sum(j => (j.EndAt!.Value - j.StartAt).TotalSeconds)))
			.ToList();
	}

	public async Task<DateTimeOffset?> GetFirstJobUsedAtAsync(string jobServiceName, CancellationToken cancellationToken = default)
	{
		var first = await _context.Set<ProcessingJobLog>()
			.AsNoTracking()
			.Where(j => j.ServiceName == jobServiceName)
			.MinAsync(j => (DateTime?)j.StartAt, cancellationToken);

		return first.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(first.Value, DateTimeKind.Utc)) : null;
	}

	public async Task<IReadOnlyList<MonthlyModelUsageRow>> GetMonthlyAiModelUsageAsync(
		int year,
		IReadOnlyCollection<int> months,
		CancellationToken cancellationToken = default)
	{
		var (from, to) = GetRange(year, months);

		var messages = await _context.Set<Message>()
			.AsNoTracking()
			.Where(m => m.ModelId != null && m.CreatedAt >= from && m.CreatedAt < to)
			.Select(m => new { m.CreatedAt, m.ModelId, m.InputTokens, m.OutputTokens })
			.ToListAsync(cancellationToken);

		return messages
			.Where(m => months.Contains(m.CreatedAt.Month))
			.GroupBy(m => new { m.CreatedAt.Month, m.ModelId })
			.Select(g => new MonthlyModelUsageRow(
				g.Key.Month,
				g.Key.ModelId!,
				g.Count(),
				g.Sum(x => (long)(x.InputTokens ?? 0)),
				g.Sum(x => (long)(x.OutputTokens ?? 0))))
			.ToList();
	}

	public async Task<DateTimeOffset?> GetFirstAiUsedAtAsync(CancellationToken cancellationToken = default)
	{
		var first = await _context.Set<Message>()
			.AsNoTracking()
			.Where(m => m.ModelId != null)
			.MinAsync(m => (DateTime?)m.CreatedAt, cancellationToken);

		return first.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(first.Value, DateTimeKind.Utc)) : null;
	}

	private static (DateTime From, DateTime To) GetRange(int year, IReadOnlyCollection<int> months)
	{
		var from = new DateTime(year, months.Min(), 1, 0, 0, 0, DateTimeKind.Utc);
		var to = new DateTime(year, months.Max(), 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
		return (from, to);
	}
}
