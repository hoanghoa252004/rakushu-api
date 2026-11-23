using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report;

namespace Rakushu.Domain.Entities.Curation.IssueCategory;

public sealed class IssueCategory : AggregateRoot<IssueCategoryId>
{
	public TargetType TargetType { get; private set; }
	public Guid TargetId { get; private set; }
	public string Category { get; private set; } = null!;
	public int? StartCharIndex { get; private set; }
	public int? EndCharIndex { get; private set; }
	public int CurrentCount { get; private set; }
	public int Threshold { get; private set; } = 3;
	public DateTimeOffset LastReportedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	private IssueCategory() { }
	private IssueCategory(IssueCategoryId id) : base(id) { }

	public static Result<IssueCategory> Create(
		TargetType targetType,
		Guid targetId,
		string category,
		DateTimeOffset now,
		int threshold = 3,
		int? startCharIndex = null,
		int? endCharIndex = null)
	{
		var validation = Validate(targetId, category, startCharIndex, endCharIndex);
		if (validation.IsFailure)
			return Result.Failure<IssueCategory>(validation.Error);

		return Result.Success(new IssueCategory(IssueCategoryId.Create())
		{
			TargetType = targetType,
			TargetId = targetId,
			Category = category.Trim(),
			StartCharIndex = startCharIndex,
			EndCharIndex = endCharIndex,
			CurrentCount = 1,
			Threshold = Math.Max(1, threshold),
			LastReportedAt = now,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public bool IncrementAndCheckThreshold(DateTimeOffset now)
	{
		CurrentCount++;
		LastReportedAt = now;
		UpdatedAt = now;
		return CurrentCount >= Threshold;
	}

	public void DecrementCount(DateTimeOffset now)
	{
		if (CurrentCount > 0)
		{
			CurrentCount--;
			UpdatedAt = now;
		}
	}

	public void ResetCount(DateTimeOffset now)
	{
		CurrentCount = 0;
		UpdatedAt = now;
	}

	private static Result Validate(
		Guid targetId,
		string category,
		int? startCharIndex,
		int? endCharIndex)
	{
		if (targetId == Guid.Empty)
			return Result.Failure(IssueCategoryErrors.InvalidTargetId);

		if (string.IsNullOrWhiteSpace(category))
			return Result.Failure(IssueCategoryErrors.EmptyCategory);

		if (startCharIndex.HasValue != endCharIndex.HasValue)
			return Result.Failure(IssueCategoryErrors.InvalidCharRange);

		if (startCharIndex.HasValue && endCharIndex.HasValue &&
			(startCharIndex.Value < 0 || endCharIndex.Value <= startCharIndex.Value))
			return Result.Failure(IssueCategoryErrors.InvalidCharRange);

		return Result.Success();
	}
}
