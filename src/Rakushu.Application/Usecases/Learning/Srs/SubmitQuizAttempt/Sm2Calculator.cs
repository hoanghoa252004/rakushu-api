using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

public sealed record Sm2CalculationResult(
	int Repetitions,
	int Lapses,
	int IntervalDays,
	double EasinessFactor,
	SrsStatus NewStatus,
	DateTimeOffset DueAt);

public static class Sm2Calculator
{
	public static Sm2CalculationResult Calculate(
		int lastRepetitions,
		int lastInterval,
		double lastFactor,
		int lastLapses,
		int quality,
		DateTimeOffset now)
	{
		int q = Math.Clamp(quality, 1, 5);
		double currentFactor = Math.Max(1.3, lastFactor <= 0 ? 2.5 : lastFactor);

		// q < 3 means failed (1: unanswered, 2: wrong)
		if (q < 3)
		{
			int lapses = lastLapses + 1;
			int interval = 1;
			int repetitions = 0;
			// Reduce factor slightly on failure
			double newFactor = Math.Max(1.3, currentFactor - 0.2);
			DateTimeOffset dueAt = now.AddDays(interval);

			return new Sm2CalculationResult(
				repetitions,
				lapses,
				interval,
				newFactor,
				SrsStatus.Learning,
				dueAt);
		}

		// q >= 3 means passed (3: slow, 4: normal, 5: fast)
		int newRepetitions = lastRepetitions + 1;
		int newLapses = lastLapses;

		double factorDelta = 0.1 - (5 - q) * (0.08 + (5 - q) * 0.02);
		double calculatedFactor = Math.Max(1.3, currentFactor + factorDelta);

		int newInterval;
		if (newRepetitions == 1)
		{
			newInterval = 1;
		}
		else if (newRepetitions == 2)
		{
			newInterval = 6;
		}
		else
		{
			int baseInterval = lastInterval <= 0 ? 1 : lastInterval;
			newInterval = Math.Max(1, (int)Math.Round(baseInterval * calculatedFactor));
		}

		DateTimeOffset calculatedDueAt = now.AddDays(newInterval);

		// Per diagram: Learner masters word when repetitions >= 3
		SrsStatus newStatus = newRepetitions >= 3 ? SrsStatus.Review : SrsStatus.Learning;

		return new Sm2CalculationResult(
			newRepetitions,
			newLapses,
			newInterval,
			calculatedFactor,
			newStatus,
			calculatedDueAt);
	}
}
