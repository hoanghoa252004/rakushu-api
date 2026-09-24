namespace Rakushu.Application.Usecases.Payment.GetMyPayments;

public sealed record PaymentSummaryDto
{
	public Guid Id { get; init; }
	public string OrderCode { get; init; } = string.Empty;
	public Guid PlanId { get; init; }
	public string PlanName { get; init; } = string.Empty;
	public decimal Amount { get; init; }
	public string Currency { get; init; } = string.Empty;
	public string Status { get; init; } = string.Empty;
	public string? Description { get; init; }
	public DateTimeOffset? CompletedAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
	public DateTimeOffset ExpiresAt { get; init; }

	public PaymentSummaryDto() { }

	public PaymentSummaryDto(
		Guid id,
		string orderCode,
		Guid planId,
		string planName,
		decimal amount,
		string currency,
		string status,
		string? description,
		DateTimeOffset? completedAt,
		DateTimeOffset createdAt,
		DateTimeOffset expiresAt)
	{
		Id = id;
		OrderCode = orderCode;
		PlanId = planId;
		PlanName = planName;
		Amount = amount;
		Currency = currency;
		Status = status;
		Description = description;
		CompletedAt = completedAt;
		CreatedAt = createdAt;
		ExpiresAt = expiresAt;
	}
}
