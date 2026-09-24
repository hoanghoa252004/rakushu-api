namespace Rakushu.Application.Usecases.Subscription.GetSubscriptions;

public sealed record SubscriptionAdminDto
{
	public Guid Id { get; init; }
	public Guid UserId { get; init; }
	public string? UserEmail { get; init; }
	public Guid PlanId { get; init; }
	public string PlanName { get; init; } = string.Empty;
	public string Status { get; init; } = string.Empty;
	public DateTimeOffset StartDate { get; init; }
	public DateTimeOffset EndDate { get; init; }
	public DateTimeOffset? CanceledAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }

	public SubscriptionAdminDto() { }

	public SubscriptionAdminDto(
		Guid id,
		Guid userId,
		string? userEmail,
		Guid planId,
		string planName,
		string status,
		DateTimeOffset startDate,
		DateTimeOffset endDate,
		DateTimeOffset? canceledAt,
		DateTimeOffset createdAt)
	{
		Id = id;
		UserId = userId;
		UserEmail = userEmail;
		PlanId = planId;
		PlanName = planName;
		Status = status;
		StartDate = startDate;
		EndDate = endDate;
		CanceledAt = canceledAt;
		CreatedAt = createdAt;
	}
}
