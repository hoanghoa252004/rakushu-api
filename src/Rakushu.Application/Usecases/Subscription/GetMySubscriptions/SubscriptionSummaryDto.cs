namespace Rakushu.Application.Usecases.Subscription.GetMySubscriptions;

public sealed record SubscriptionSummaryDto
{
	public Guid Id { get; init; }
	public Guid PlanId { get; init; }
	public string PlanName { get; init; } = string.Empty;
	public string Status { get; init; } = string.Empty;
	public DateTimeOffset StartDate { get; init; }
	public DateTimeOffset EndDate { get; init; }
	public DateTimeOffset? CanceledAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }

	public SubscriptionSummaryDto() { }

	public SubscriptionSummaryDto(
		Guid id,
		Guid planId,
		string planName,
		string status,
		DateTimeOffset startDate,
		DateTimeOffset endDate,
		DateTimeOffset? canceledAt,
		DateTimeOffset createdAt)
	{
		Id = id;
		PlanId = planId;
		PlanName = planName;
		Status = status;
		StartDate = startDate;
		EndDate = endDate;
		CanceledAt = canceledAt;
		CreatedAt = createdAt;
	}
}
