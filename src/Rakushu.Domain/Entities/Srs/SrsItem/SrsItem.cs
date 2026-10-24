using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMeaning;
using Rakushu.Domain.Entities.Material.LearningUnit;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsItem : AggregateRoot<SrsItemId>
{
	// MAIN PROPERTIES----------
	public UserId UserId { get; private set; } = null!;
	public KnowledgeMeaningId KnowledgeMeaningId { get; private set; } = null!;
	public LearningUnitId? SourceLearningUnitId { get; private set; }
	public string Expression { get; private set; } = null!;
	public string? Reading { get; private set; }
	public string Meaning { get; private set; } = null!;
	public string? Description { get; private set; }
	public SrsItemStatus Status { get; private set; } = SrsItemStatus.New;

	// EMBEDDED SM-2 METRICS----------
	public DateTimeOffset DueAt { get; private set; }
	public int IntervalDays { get; private set; }
	public double EasinessFactor { get; private set; } = 2.5;
	public int Repetitions { get; private set; }
	public int Lapses { get; private set; }
	public DateTimeOffset? LastReviewAt { get; private set; }

	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	public User User { get; private set; } = null!;
	public KnowledgeMeaning KnowledgeMeaning { get; private set; } = null!;
	public LearningUnit? SourceLearningUnit { get; private set; }

	// CONSTRUCTORS & FACTORY METHODS----------
	private SrsItem() { }
	private SrsItem(SrsItemId id) : base(id) { }

	public static Result<SrsItem> Create(
		UserId userId,
		KnowledgeMeaningId knowledgeMeaningId,
		string expression,
		string? reading,
		string meaning,
		string? description,
		DateTimeOffset now,
		LearningUnitId? sourceLearningUnitId = null)
	{
		if (userId is null)
			return Result.Failure<SrsItem>(SrsItemErrors.InvalidUserId);
		if (knowledgeMeaningId is null)
			return Result.Failure<SrsItem>(SrsItemErrors.InvalidKnowledgeMeaningId);
		if (string.IsNullOrWhiteSpace(expression))
			return Result.Failure<SrsItem>(SrsItemErrors.InvalidExpression);
		if (string.IsNullOrWhiteSpace(meaning))
			return Result.Failure<SrsItem>(SrsItemErrors.InvalidMeaning);

		return Result.Success(new SrsItem(SrsItemId.Create())
		{
			UserId = userId,
			KnowledgeMeaningId = knowledgeMeaningId,
			SourceLearningUnitId = sourceLearningUnitId,
			Expression = expression.Trim(),
			Reading = string.IsNullOrWhiteSpace(reading) ? null : reading.Trim(),
			Meaning = meaning.Trim(),
			Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
			Status = SrsItemStatus.New,
			DueAt = now,
			IntervalDays = 0,
			EasinessFactor = 2.5,
			Repetitions = 0,
			Lapses = 0,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public Result Suspend(DateTimeOffset now) =>
		ChangeStatus(SrsItemStatus.Suspended, now, SrsItemStatus.New, SrsItemStatus.Learning, SrsItemStatus.Review);

	public Result Resume(DateTimeOffset now)
	{
		if (Status != SrsItemStatus.Suspended)
			return Result.Failure(SrsItemErrors.InvalidItemStatusTransition);
		Status = Repetitions == 0 ? SrsItemStatus.Learning : SrsItemStatus.Review;
		UpdatedAt = now;
		return Result.Success();
	}

	public Result Ignore(DateTimeOffset now)
	{
		if (Status == SrsItemStatus.Ignored)
			return Result.Failure(SrsItemErrors.ItemAlreadyIgnored);
		Status = SrsItemStatus.Ignored;
		UpdatedAt = now;
		return Result.Success();
	}

	public Result RecordReview(int quality, DateTimeOffset reviewedAt)
	{
		if (Status is SrsItemStatus.Suspended or SrsItemStatus.Ignored)
			return Result.Failure(SrsItemErrors.ItemNotReviewable);
		if (quality is < 0 or > 5)
			return Result.Failure(SrsItemErrors.InvalidReviewRating);
		if (LastReviewAt is not null && reviewedAt < LastReviewAt)
			return Result.Failure(SrsItemErrors.InvalidItemStatusTransition);

		if (quality < 3)
		{
			// Failed recall resets the repetition sequence and interval,
			// but leaves the easiness factor unchanged.
			Repetitions = 0;
			Lapses++;
			IntervalDays = 1;
			Status = SrsItemStatus.Learning;
		}
		else
		{
			IntervalDays = Repetitions switch
			{
				0 => 1,
				1 => 6,
				_ => Math.Max(1, (int)Math.Ceiling(Math.Max(1, IntervalDays) * EasinessFactor))
			};
			Repetitions++;
			Status = SrsItemStatus.Review;

			EasinessFactor = Math.Max(
				1.3,
				EasinessFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02)));
		}

		LastReviewAt = reviewedAt;
		DueAt = reviewedAt.AddDays(IntervalDays);
		UpdatedAt = reviewedAt;
		return Result.Success();
	}

	private Result ChangeStatus(SrsItemStatus nextStatus, DateTimeOffset now, params SrsItemStatus[] allowedCurrentStatuses)
	{
		if (!allowedCurrentStatuses.Contains(Status))
			return Result.Failure(SrsItemErrors.InvalidItemStatusTransition);
		Status = nextStatus;
		UpdatedAt = now;
		return Result.Success();
	}
}
