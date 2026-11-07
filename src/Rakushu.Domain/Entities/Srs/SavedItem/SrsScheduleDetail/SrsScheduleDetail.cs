using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

public sealed class SrsScheduleDetail : Entity<SrsScheduleDetailId>
{
	public SrsScheduleId SrsScheduleId { get; private set; } = null!;
	public QuizAttemptItemId? QuizAttemptItemId { get; private set; }
	public FlashcardSessionItemId? FlashcardSessionItemId { get; private set; }
	public int ReviewNumber { get; private set; }
	public int Quality { get; private set; }
	public int IntervalDays { get; private set; }
	public double EasinessFactor { get; private set; } = 2.5;
	public int Repetitions { get; private set; }
	public int Lapses { get; private set; }
	public DateTimeOffset ReviewedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public SrsSchedule.SrsSchedule SrsSchedule { get; private set; } = null!;
	public QuizAttemptItem? QuizAttemptItem { get; private set; }
	public FlashcardSessionItem? FlashcardSessionItem { get; private set; }

	private SrsScheduleDetail() { }
	private SrsScheduleDetail(SrsScheduleDetailId id) : base(id) { }

	public static Result<SrsScheduleDetail> Create(
		SrsScheduleId srsScheduleId,
		int reviewNumber,
		int quality,
		int intervalDays,
		double easinessFactor,
		int repetitions,
		int lapses,
		DateTimeOffset reviewedAt,
		QuizAttemptItemId? quizAttemptItemId = null,
		FlashcardSessionItemId? flashcardSessionItemId = null)
	{
		var validation = Validate(srsScheduleId, reviewNumber, quality);
		if (validation.IsFailure)
			return Result.Failure<SrsScheduleDetail>(validation.Error);

		return Result.Success(new SrsScheduleDetail(SrsScheduleDetailId.Create())
		{
			SrsScheduleId = srsScheduleId,
			QuizAttemptItemId = quizAttemptItemId,
			FlashcardSessionItemId = flashcardSessionItemId,
			ReviewNumber = reviewNumber,
			Quality = quality,
			IntervalDays = Math.Max(0, intervalDays),
			EasinessFactor = Math.Max(1.3, easinessFactor),
			Repetitions = Math.Max(0, repetitions),
			Lapses = Math.Max(0, lapses),
			ReviewedAt = reviewedAt
		});
	}

	private static Result Validate(SrsScheduleId srsScheduleId, int reviewNumber, int quality)
	{
		if (srsScheduleId is null || srsScheduleId.Value == Guid.Empty)
			return Result.Failure(SrsScheduleDetailErrors.InvalidSrsScheduleId);

		if (reviewNumber < 1)
			return Result.Failure(SrsScheduleDetailErrors.InvalidReviewNumber);

		if (quality is < 0 or > 5)
			return Result.Failure(SrsScheduleDetailErrors.InvalidQuality);

		return Result.Success();
	}
}
