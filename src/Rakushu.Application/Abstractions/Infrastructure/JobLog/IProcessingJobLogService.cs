using Rakushu.Domain.Entities.Processing.ProcessingJobLog;

namespace Rakushu.Application.Abstractions.Infrastructure.JobLog;

/// <summary>
/// Persists processing job logs in their own unit of work, so a log survives a rollback of the
/// business transaction that triggered the job (the external job keeps running either way).
/// </summary>
public interface IProcessingJobLogService
{
	Task<ProcessingJobLogInfo> StartAsync(
		ProcessingJobType jobType,
		ProcessingJobTargetType targetType,
		Guid targetId,
		string serviceName,
		string? input,
		CancellationToken cancellationToken = default);

	Task SetJobLabelAsync(Guid jobLogId, string jobLabel, CancellationToken cancellationToken = default);

	Task CompleteAsync(Guid jobLogId, string? output, CancellationToken cancellationToken = default);

	Task FailAsync(Guid jobLogId, string? output, CancellationToken cancellationToken = default);

	/// <summary>Returns the most recently started log of the given job type for the target, if any.</summary>
	Task<ProcessingJobLogInfo?> GetLatestAsync(
		ProcessingJobType jobType,
		Guid targetId,
		CancellationToken cancellationToken = default);

	/// <summary>Returns all logs of the given job type for the targets, newest first.</summary>
	Task<IReadOnlyList<ProcessingJobLogInfo>> GetByTargetsAsync(
		ProcessingJobType jobType,
		IReadOnlyCollection<Guid> targetIds,
		CancellationToken cancellationToken = default);

	/// <summary>Searches logs with full details, ordered by target then ascending sequence.</summary>
	Task<IReadOnlyList<ProcessingJobLogDetail>> SearchAsync(
		ProcessingJobLogFilter filter,
		CancellationToken cancellationToken = default);
}

public sealed record ProcessingJobLogFilter(
	Guid? TargetId,
	ProcessingJobTargetType? TargetType,
	ProcessingJobType? JobType,
	bool? IsSuccess,
	bool? IsFailed);

public sealed record ProcessingJobLogDetail(
	Guid Id,
	int Sequence,
	ProcessingJobType JobType,
	ProcessingJobTargetType TargetType,
	Guid TargetId,
	string ServiceName,
	string? JobLabel,
	string? Input,
	string? Output,
	bool IsSuccess,
	bool IsFailed,
	DateTime StartAt,
	DateTime? EndAt);

public static class ProcessingJobLogServiceExtensions
{
	/// <summary>
	/// Finds the current log of the job type for the target, or starts a new one when none exists.
	/// Reusing the current log keeps the external job id stable across polling runs.
	/// </summary>
	public static async Task<ProcessingJobLogInfo> GetOrStartAsync(
		this IProcessingJobLogService service,
		ProcessingJobType jobType,
		ProcessingJobTargetType targetType,
		Guid targetId,
		string serviceName,
		string? input,
		CancellationToken cancellationToken = default)
	{
		var logs = await service.GetByTargetsAsync(jobType, [targetId], cancellationToken);
		return logs.SelectCurrent()
			?? await service.StartAsync(jobType, targetType, targetId, serviceName, input, cancellationToken);
	}

	/// <summary>
	/// Picks the log that represents the job: a running one (no EndAt, not success/failed) first,
	/// then a succeeded one, then the latest failed one. Newest first within each group.
	/// </summary>
	public static ProcessingJobLogInfo? SelectCurrent(this IEnumerable<ProcessingJobLogInfo> logs)
	{
		var ordered = logs.OrderByDescending(l => l.StartAt).ToList();
		return ordered.FirstOrDefault(l => l.IsRunning)
			?? ordered.FirstOrDefault(l => l.IsSuccess && l.EndAt is not null)
			?? ordered.FirstOrDefault(l => l.IsFailed && l.EndAt is not null);
	}
}

public sealed record ProcessingJobLogInfo(
	Guid Id,
	ProcessingJobType JobType,
	Guid TargetId,
	int Sequence,
	string? JobLabel,
	string? Input,
	bool IsSuccess,
	bool IsFailed,
	DateTime StartAt,
	DateTime? EndAt)
{
	public bool IsFinished => EndAt is not null && (IsSuccess || IsFailed);
	public bool IsRunning => !IsFinished;
}
