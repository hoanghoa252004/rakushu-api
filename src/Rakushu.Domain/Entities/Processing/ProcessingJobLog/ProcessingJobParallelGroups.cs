namespace Rakushu.Domain.Entities.Processing.ProcessingJobLog;

/// <summary>
/// Groups job types that run in parallel for the same target and therefore share a sequence number.
/// </summary>
public static class ProcessingJobParallelGroups
{
	/// <summary>Returns the parallel group key of the job type, or null when it runs alone.</summary>
	public static string? GetGroup(ProcessingJobType jobType) => jobType switch
	{
		ProcessingJobType.LanguageModeration or ProcessingJobType.ContentModeration => "Moderation",
		// Clip jobs of all role lines of a material are dispatched together.
		ProcessingJobType.ShadowingAudioClip => "ShadowingAudioClip",
		_ => null
	};

	/// <summary>
	/// Whether a new job may join the jobs currently holding the latest sequence of the target.
	/// Different job types of a group join each other once; same-type jobs only join for groups
	/// that dispatch many jobs of one type at once (shadowing clips).
	/// </summary>
	public static bool CanJoin(ProcessingJobType newJobType, IReadOnlyCollection<ProcessingJobType> latestJobTypes)
	{
		var group = GetGroup(newJobType);
		if (group is null || latestJobTypes.Count == 0)
			return false;

		if (latestJobTypes.Any(t => GetGroup(t) != group))
			return false;

		return newJobType == ProcessingJobType.ShadowingAudioClip
			|| !latestJobTypes.Contains(newJobType);
	}
}
