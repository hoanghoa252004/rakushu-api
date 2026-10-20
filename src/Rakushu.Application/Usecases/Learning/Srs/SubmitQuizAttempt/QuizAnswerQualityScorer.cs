namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

/// <summary>
/// Scores multiple-choice recall deterministically. Timing is only used to distinguish correct answers.
/// Missing or implausible timing never awards the maximum quality.
/// </summary>
public static class QuizAnswerQualityScorer
{
	public const int MinimumPlausibleResponseMs = 500;
	public const int MaximumPlausibleResponseMs = 120_000;
	public const int FastResponseThresholdMs = 3_000;
	public const int NormalResponseThresholdMs = 8_000;

	public static int? NormalizeElapsedMs(int? elapsedMs)
	{
		if (elapsedMs is null or < 0 || elapsedMs > MaximumPlausibleResponseMs)
			return null;

		return elapsedMs;
	}

	public static int Calculate(bool isCorrect, int? elapsedMs, bool hasAnswer = true)
	{
		if (!hasAnswer)
			return 0;

		if (!isCorrect)
			return 1;

		if (elapsedMs is null ||
			elapsedMs < MinimumPlausibleResponseMs ||
			elapsedMs > MaximumPlausibleResponseMs)
			return 3;

		if (elapsedMs < FastResponseThresholdMs)
			return 5;

		if (elapsedMs < NormalResponseThresholdMs)
			return 4;

		return 3;
	}
}
