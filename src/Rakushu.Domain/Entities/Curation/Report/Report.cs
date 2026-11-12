using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Curation.Report.IssueDetail;

namespace Rakushu.Domain.Entities.Curation.Report;

public sealed class Report : AggregateRoot<ReportId>
{
	public TargetType TargetType { get; private set; }
	public Guid TargetId { get; private set; }
	public string Category { get; private set; } = null!;
	public ReportStatus Status { get; private set; } = ReportStatus.Pending;
	public string? CuratorNotes { get; private set; }
	public DateTimeOffset? ResolvedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	private readonly List<IssueDetail.IssueDetail> _details = [];
	public IReadOnlyCollection<IssueDetail.IssueDetail> Details => _details.AsReadOnly();

	private Report() { }
	private Report(ReportId id) : base(id) { }

	public static Result<Report> Create(
		TargetType targetType,
		Guid targetId,
		string category,
		DateTimeOffset now)
	{
		var validation = Validate(targetId, category);
		if (validation.IsFailure)
			return Result.Failure<Report>(validation.Error);

		return Result.Success(new Report(ReportId.Create())
		{
			TargetType = targetType,
			TargetId = targetId,
			Category = category.Trim(),
			Status = ReportStatus.Pending,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void MarkInReview(DateTimeOffset now)
	{
		Status = ReportStatus.InReview;
		UpdatedAt = now;
	}

	public void Resolve(string curatorNotes, DateTimeOffset now)
	{
		Status = ReportStatus.Resolved;
		CuratorNotes = curatorNotes?.Trim();
		ResolvedAt = now;
		UpdatedAt = now;
	}

	public void Dismiss(string reason, DateTimeOffset now)
	{
		Status = ReportStatus.Dismissed;
		CuratorNotes = reason?.Trim();
		ResolvedAt = now;
		UpdatedAt = now;
	}

	private static Result Validate(Guid targetId, string category)
	{
		if (targetId == Guid.Empty)
			return Result.Failure(ReportErrors.InvalidTargetId);

		if (string.IsNullOrWhiteSpace(category))
			return Result.Failure(ReportErrors.EmptyCategory);

		return Result.Success();
	}
}
