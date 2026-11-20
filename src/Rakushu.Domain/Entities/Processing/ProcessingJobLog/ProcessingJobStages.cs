namespace Rakushu.Domain.Entities.Processing.ProcessingJobLog;

/// <summary>
/// Fixed pipeline stages per job type. Sequence is the stage order within a target (parallel jobs share it);
/// parent job types are the jobs that must run before, used to link a log to the run it depends on.
/// </summary>
public static class ProcessingJobStages
{
	public sealed record Stage(int Sequence, IReadOnlyList<ProcessingJobType> ParentJobTypes);

	private static readonly IReadOnlyList<ProcessingJobType> None = [];

	public static Stage Get(ProcessingJobType jobType) => jobType switch
	{
		// Video pipeline.
		ProcessingJobType.YoutubeImport => new(1, None),
		ProcessingJobType.LanguageModeration or ProcessingJobType.ContentModeration => new(2, None),
		ProcessingJobType.Transcription => new(3, None),
		// Translation chunk sau nhận parent tường minh là chunk trước; chunk đầu không có parent.
		ProcessingJobType.Nlp or ProcessingJobType.Translation => new(4, None),
		ProcessingJobType.LearningUnitMatching => new(5, [ProcessingJobType.Nlp]),

		// Shadowing material.
		ProcessingJobType.ShadowingAudioClip => new(1, None),

		// Shadowing attempt detail.
		ProcessingJobType.ShadowingAudioNormalization => new(1, None),
		ProcessingJobType.PronunciationAssessment or ProcessingJobType.PitchAssessment
			=> new(2, [ProcessingJobType.ShadowingAudioNormalization]),

		_ => throw new ArgumentOutOfRangeException(nameof(jobType), jobType, "Unknown processing job type.")
	};
}
