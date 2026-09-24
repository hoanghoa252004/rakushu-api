namespace Rakushu.Application.Usecases.Payment.GetPayments;

public sealed record PaymentAdminDto
{
	public Guid Id { get; init; }
	public Guid UserId { get; init; }
	public string? UserEmail { get; init; }
	public Guid PlanId { get; init; }
	public string PlanName { get; init; } = string.Empty;
	public Guid? SubscriptionId { get; init; }
	public string OrderCode { get; init; } = string.Empty;
	public decimal Amount { get; init; }
	public string Currency { get; init; } = string.Empty;
	public string Status { get; init; } = string.Empty;
	public string? Description { get; init; }
	public DateTimeOffset? CompletedAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public DateTimeOffset ExpiresAt { get; init; }

	public PaymentAdminDto() { }

	public PaymentAdminDto(
		Guid id,
		Guid userId,
		string? userEmail,
		Guid planId,
		string planName,
		Guid? subscriptionId,
		string orderCode,
		decimal amount,
		string currency,
		string status,
		string? description,
		DateTimeOffset? completedAt,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt)
	{
		Id = id;
		UserId = userId;
		UserEmail = userEmail;
		PlanId = planId;
		PlanName = planName;
		SubscriptionId = subscriptionId;
		OrderCode = orderCode;
		Amount = amount;
		Currency = currency;
		Status = status;
		Description = description;
		CompletedAt = completedAt;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}
}
