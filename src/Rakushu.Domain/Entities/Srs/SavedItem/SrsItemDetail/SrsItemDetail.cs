using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;

public sealed class SrsItemDetail : Entity<SrsItemDetailId>
{
	public SavedItemId SavedItemId { get; private set; } = null!;
	public QuizAttemptItemId? QuizAttemptItemId { get; private set; }
	public int ReviewNumber { get; private set; }
	public int Quality { get; private set; }
	public int IntervalDays { get; private set; }
	public double EasinessFactor { get; private set; } = 2.5;
	public int Repetitions { get; private set; }
	public int Lapses { get; private set; }
	public DateTimeOffset ReviewedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public SavedItem SavedItem { get; private set; } = null!;
	public QuizAttemptItem? QuizAttemptItem { get; private set; }

	private SrsItemDetail() { }
	private SrsItemDetail(SrsItemDetailId id) : base(id) { }

	public static Result<SrsItemDetail> Create(
		SavedItemId savedItemId,
		int reviewNumber,
		int quality,
		int intervalDays,
		double easinessFactor,
		int repetitions,
		int lapses,
		DateTimeOffset reviewedAt,
		QuizAttemptItemId? quizAttemptItemId = null)
	{
		var validation = Validate(savedItemId, reviewNumber, quality);
		if (validation.IsFailure)
			return Result.Failure<SrsItemDetail>(validation.Error);

		return Result.Success(new SrsItemDetail(SrsItemDetailId.Create())
		{
			SavedItemId = savedItemId,
			QuizAttemptItemId = quizAttemptItemId,
			ReviewNumber = reviewNumber,
			Quality = quality,
			IntervalDays = Math.Max(0, intervalDays),
			EasinessFactor = Math.Max(1.3, easinessFactor),
			Repetitions = Math.Max(0, repetitions),
			Lapses = Math.Max(0, lapses),
			ReviewedAt = reviewedAt
		});
	}

	private static Result Validate(SavedItemId savedItemId, int reviewNumber, int quality)
	{
		if (savedItemId is null || savedItemId.Value == Guid.Empty)
			return Result.Failure(SrsItemDetailErrors.InvalidSavedItemId);

		if (reviewNumber < 1)
			return Result.Failure(SrsItemDetailErrors.InvalidReviewNumber);

		if (quality is < 0 or > 5)
			return Result.Failure(SrsItemDetailErrors.InvalidQuality);

		return Result.Success();
	}
}
