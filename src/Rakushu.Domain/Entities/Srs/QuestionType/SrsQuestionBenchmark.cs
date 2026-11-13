namespace Rakushu.Domain.Entities.Srs.QuestionType;

public static class SrsQuestionBenchmark
{
	public const int DefaultExpectedDurationSeconds = 15;

	public static int CalculateQuality(
		bool isCorrect,
		int timeSpentMs,
		int? expectedDurationSeconds)
	{
		if (!isCorrect)
			return 1;

		int expectedSeconds = expectedDurationSeconds is > 0
			? expectedDurationSeconds.Value
			: DefaultExpectedDurationSeconds;

		int expectedMs = expectedSeconds * 1000;
		int fastThresholdMs = expectedMs / 3;

		return timeSpentMs > expectedMs ? 3
			: timeSpentMs >= fastThresholdMs ? 4
			: 5;
	}
}
