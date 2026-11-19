using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Infrastructure.JobLog;
using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Persistence.Repositories;

/// <summary>
/// Each operation uses its own DbContext scope and commits immediately, independent of the
/// caller's business transaction.
/// </summary>
internal sealed class ProcessingJobLogService(IServiceScopeFactory scopeFactory, ISystemClock clock)
	: IProcessingJobLogService
{
	public async Task<ProcessingJobLogInfo> StartAsync(
		ProcessingJobType jobType,
		ProcessingJobTargetType targetType,
		Guid targetId,
		string serviceName,
		string? input,
		CancellationToken cancellationToken = default)
	{
		await using var scope = scopeFactory.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();

		var sequence = await GetNextSequenceAsync(db, jobType, targetType, targetId, cancellationToken);
		var log = ProcessingJobLog.Start(jobType, targetType, targetId, sequence, serviceName, input, clock.UtcNow.UtcDateTime);

		db.ProcessingJobLogs.Add(log);
		await db.SaveChangesAsync(cancellationToken);
		return ToInfo(log);
	}

	private static async Task<int> GetNextSequenceAsync(
		RakushuDbContext db,
		ProcessingJobType jobType,
		ProcessingJobTargetType targetType,
		Guid targetId,
		CancellationToken cancellationToken)
	{
		var targetLogs = db.ProcessingJobLogs
			.Where(l => l.TargetType == targetType && l.TargetId == targetId);

		var maxSequence = await targetLogs.MaxAsync(l => (int?)l.Sequence, cancellationToken);
		if (maxSequence is null)
			return 1;

		var latestJobTypes = await targetLogs
			.Where(l => l.Sequence == maxSequence.Value)
			.Select(l => l.JobType)
			.Distinct()
			.ToListAsync(cancellationToken);

		return ProcessingJobParallelGroups.CanJoin(jobType, latestJobTypes)
			? maxSequence.Value
			: maxSequence.Value + 1;
	}

	public Task SetJobLabelAsync(Guid jobLogId, string jobLabel, CancellationToken cancellationToken = default)
		=> UpdateAsync(jobLogId, log => log.SetJobLabel(jobLabel), cancellationToken);

	public Task CompleteAsync(Guid jobLogId, string? output, CancellationToken cancellationToken = default)
		=> UpdateAsync(jobLogId, log => log.Complete(output, clock.UtcNow.UtcDateTime), cancellationToken);

	public Task FailAsync(Guid jobLogId, string? output, CancellationToken cancellationToken = default)
		=> UpdateAsync(jobLogId, log => log.Fail(output, clock.UtcNow.UtcDateTime), cancellationToken);

	public async Task<ProcessingJobLogInfo?> GetLatestAsync(
		ProcessingJobType jobType,
		Guid targetId,
		CancellationToken cancellationToken = default)
	{
		await using var scope = scopeFactory.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();
		var log = await db.ProcessingJobLogs
			.AsNoTracking()
			.Where(l => l.JobType == jobType && l.TargetId == targetId)
			.OrderByDescending(l => l.StartAt)
			.FirstOrDefaultAsync(cancellationToken);
		return log is null ? null : ToInfo(log);
	}

	public async Task<IReadOnlyList<ProcessingJobLogInfo>> GetByTargetsAsync(
		ProcessingJobType jobType,
		IReadOnlyCollection<Guid> targetIds,
		CancellationToken cancellationToken = default)
	{
		if (targetIds.Count == 0)
			return [];

		await using var scope = scopeFactory.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();
		var logs = await db.ProcessingJobLogs
			.AsNoTracking()
			.Where(l => l.JobType == jobType && targetIds.Contains(l.TargetId))
			.OrderByDescending(l => l.StartAt)
			.ToListAsync(cancellationToken);
		return logs.Select(ToInfo).ToList();
	}

	public async Task<IReadOnlyList<ProcessingJobLogDetail>> SearchAsync(
		ProcessingJobLogFilter filter,
		CancellationToken cancellationToken = default)
	{
		await using var scope = scopeFactory.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();

		var query = db.ProcessingJobLogs.AsNoTracking();
		if (filter.TargetId is not null)
			query = query.Where(l => l.TargetId == filter.TargetId);
		if (filter.TargetType is not null)
			query = query.Where(l => l.TargetType == filter.TargetType);
		if (filter.JobType is not null)
			query = query.Where(l => l.JobType == filter.JobType);
		if (filter.IsSuccess is not null)
			query = query.Where(l => l.IsSuccess == filter.IsSuccess);
		if (filter.IsFailed is not null)
			query = query.Where(l => l.IsFailed == filter.IsFailed);

		return await query
			.OrderBy(l => l.TargetType)
			.ThenBy(l => l.TargetId)
			.ThenBy(l => l.Sequence)
			.ThenBy(l => l.StartAt)
			.Select(l => new ProcessingJobLogDetail(
				l.Id.Value,
				l.Sequence,
				l.JobType,
				l.TargetType,
				l.TargetId,
				l.ServiceName,
				l.JobLabel,
				l.Input,
				l.Output,
				l.IsSuccess,
				l.IsFailed,
				l.StartAt,
				l.EndAt))
			.ToListAsync(cancellationToken);
	}

	private async Task UpdateAsync(Guid jobLogId, Action<ProcessingJobLog> update, CancellationToken cancellationToken)
	{
		await using var scope = scopeFactory.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<RakushuDbContext>();
		var id = ProcessingJobLogId.From(jobLogId);
		var log = await db.ProcessingJobLogs.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
		if (log is null)
			return;

		update(log);
		await db.SaveChangesAsync(cancellationToken);
	}

	private static ProcessingJobLogInfo ToInfo(ProcessingJobLog log) => new(
		log.Id.Value,
		log.JobType,
		log.TargetId,
		log.Sequence,
		log.JobLabel,
		log.Input,
		log.IsSuccess,
		log.IsFailed,
		log.StartAt,
		log.EndAt);
}
