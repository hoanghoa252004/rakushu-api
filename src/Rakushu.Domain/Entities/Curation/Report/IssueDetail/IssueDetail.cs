using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Curation.Report.IssueDetail;

public sealed class IssueDetail : Entity<IssueDetailId>
{
	public UserId UserId { get; private set; } = null!;
	public TargetType TargetType { get; private set; }
	public Guid TargetId { get; private set; }
	public string Category { get; private set; } = null!;
	public string? LearnerFeedback { get; private set; }
	public string? TargetSnapshot { get; private set; }
	public int? StartCharIndex { get; private set; }
	public int? EndCharIndex { get; private set; }
	public ReportId? ReportId { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public Report? Report { get; private set; }

	private IssueDetail() { }
	private IssueDetail(IssueDetailId id) : base(id) { }

	public static Result<IssueDetail> Create(
		UserId userId,
		TargetType targetType,
		Guid targetId,
		string category,
		DateTimeOffset now,
		string? learnerFeedback = null,
		ReportId? reportId = null,
		string? targetSnapshot = null,
		int? startCharIndex = null,
		int? endCharIndex = null)
	{
		var validation = Validate(userId, targetId, category, startCharIndex, endCharIndex);
		if (validation.IsFailure)
			return Result.Failure<IssueDetail>(validation.Error);

		return Result.Success(new IssueDetail(IssueDetailId.Create())
		{
			UserId = userId,
			TargetType = targetType,
			TargetId = targetId,
			Category = category.Trim(),
			LearnerFeedback = string.IsNullOrWhiteSpace(learnerFeedback) ? null : learnerFeedback.Trim(),
			TargetSnapshot = string.IsNullOrWhiteSpace(targetSnapshot) ? null : targetSnapshot.Trim(),
			StartCharIndex = startCharIndex,
			EndCharIndex = endCharIndex,
			ReportId = reportId,
			CreatedAt = now
		});
	}

	public void AssignToReport(ReportId reportId)
	{
		ReportId = reportId;
	}

	private static Result Validate(
		UserId userId,
		Guid targetId,
		string category,
		int? startCharIndex,
		int? endCharIndex)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure(IssueDetailErrors.InvalidUserId);

		if (targetId == Guid.Empty)
			return Result.Failure(IssueDetailErrors.InvalidTargetId);

		if (string.IsNullOrWhiteSpace(category))
			return Result.Failure(IssueDetailErrors.EmptyCategory);

		if (startCharIndex.HasValue != endCharIndex.HasValue)
			return Result.Failure(IssueDetailErrors.InvalidCharRange);

		if (startCharIndex.HasValue && endCharIndex.HasValue &&
			(startCharIndex.Value < 0 || endCharIndex.Value <= startCharIndex.Value))
			return Result.Failure(IssueDetailErrors.InvalidCharRange);

		return Result.Success();
	}
}
