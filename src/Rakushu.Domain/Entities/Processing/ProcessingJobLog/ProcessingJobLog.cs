using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Processing.ProcessingJobLog;

/// <summary>
/// Tracks one processing run of a job (internal or on an external service) for a target entity.
/// The log id is also used as the job identifier on external services.
/// </summary>
public sealed class ProcessingJobLog : Entity<ProcessingJobLogId>
{
	public ProcessingJobType JobType { get; private set; }
	public ProcessingJobTargetType TargetType { get; private set; }
	public Guid TargetId { get; private set; }

	/// <summary>Order of the job within its target (1, 2, 3...). Parallel jobs share the same sequence.</summary>
	public int Sequence { get; private set; }
	public string ServiceName { get; private set; } = null!;
	public string? JobLabel { get; private set; }
	public string? Input { get; private set; }
	public string? Output { get; private set; }
	public bool IsSuccess { get; private set; }
	public bool IsFailed { get; private set; }
	public DateTime StartAt { get; private set; }
	public DateTime? EndAt { get; private set; }

	/// <summary>The job reached a terminal state (success or failure) and has an end time.</summary>
	public bool IsFinished => EndAt is not null && (IsSuccess || IsFailed);

	private ProcessingJobLog() { }

	public static ProcessingJobLog Start(
		ProcessingJobType jobType,
		ProcessingJobTargetType targetType,
		Guid targetId,
		int sequence,
		string serviceName,
		string? input,
		DateTime now)
	{
		if (string.IsNullOrWhiteSpace(serviceName))
			throw new ArgumentException("Service name is required.", nameof(serviceName));
		if (sequence < 1)
			throw new ArgumentOutOfRangeException(nameof(sequence), "Sequence must start from 1.");

		return new ProcessingJobLog
		{
			Id = ProcessingJobLogId.Create(),
			JobType = jobType,
			TargetType = targetType,
			TargetId = targetId,
			Sequence = sequence,
			ServiceName = serviceName,
			Input = input,
			IsSuccess = false,
			IsFailed = false,
			StartAt = now
		};
	}

	public void SetJobLabel(string? jobLabel)
	{
		if (!string.IsNullOrWhiteSpace(jobLabel))
			JobLabel = jobLabel;
	}

	public void Complete(string? output, DateTime now)
	{
		if (IsFinished)
			return;

		Output = output;
		IsSuccess = true;
		IsFailed = false;
		EndAt = now;
	}

	public void Fail(string? output, DateTime now)
	{
		if (IsFinished)
			return;

		Output = output;
		IsSuccess = false;
		IsFailed = true;
		EndAt = now;
	}
}
