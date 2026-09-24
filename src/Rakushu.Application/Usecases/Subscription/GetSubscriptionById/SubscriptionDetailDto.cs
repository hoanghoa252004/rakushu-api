namespace Rakushu.Application.Usecases.Subscription.GetSubscriptionById;

public sealed record SubscriptionDetailDto(
	Guid Id,
	Guid UserId,
	string? UserEmail,
	Guid PlanId,
	string PlanName,
	string Status,
	DateTimeOffset StartDate,
	DateTimeOffset EndDate,
	DateTimeOffset CurrentPeriodStart,
	DateTimeOffset CurrentPeriodEnd,
	DateTimeOffset? CanceledAt,
	DateTimeOffset CreatedAt,
	List<SubscriptionUsageDto> Usages,
	List<SubscriptionPaymentDto> Payments
);

public sealed record SubscriptionUsageDto
{
	public Guid FeatureId { get; init; }
	public string FeatureName { get; init; } = string.Empty;
	public string FeatureCode { get; init; } = string.Empty;
	public string FeatureType { get; init; } = string.Empty;
	public int? LimitValue { get; init; }
	public int UsedValue { get; init; }
	public DateTimeOffset PeriodStart { get; init; }
	public DateTimeOffset PeriodEnd { get; init; }

	public SubscriptionUsageDto() { }

	public SubscriptionUsageDto(
		Guid featureId,
		string featureName,
		string featureCode,
		string featureType,
		int? limitValue,
		int usedValue,
		DateTimeOffset periodStart,
		DateTimeOffset periodEnd)
	{
		FeatureId = featureId;
		FeatureName = featureName;
		FeatureCode = featureCode;
		FeatureType = featureType;
		LimitValue = limitValue;
		UsedValue = usedValue;
		PeriodStart = periodStart;
		PeriodEnd = periodEnd;
	}
}

public sealed record SubscriptionPaymentDto
{
	public Guid Id { get; init; }
	public string OrderCode { get; init; } = string.Empty;
	public decimal Amount { get; init; }
	public string Currency { get; init; } = string.Empty;
	public string Status { get; init; } = string.Empty;
	public DateTimeOffset? CompletedAt { get; init; }
	public DateTimeOffset CreatedAt { get; init; }

	public SubscriptionPaymentDto() { }

	public SubscriptionPaymentDto(
		Guid id,
		string orderCode,
		decimal amount,
		string currency,
		string status,
		DateTimeOffset? completedAt,
		DateTimeOffset createdAt)
	{
		Id = id;
		OrderCode = orderCode;
		Amount = amount;
		Currency = currency;
		Status = status;
		CompletedAt = completedAt;
		CreatedAt = createdAt;
	}
}
