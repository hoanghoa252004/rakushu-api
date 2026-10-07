using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;
using Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptionById;

public sealed record SubscriptionUsageDto(
	Guid Id,
	DateTimeOffset PeriodStart,
	DateTimeOffset PeriodEnd,
	int MaxValue,
	int UsedValue,
	bool IsOverLimit,
	bool IsExpired,
	bool IsCanceled,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	FeatureDto Feature
);

public sealed record SubscriptionDetailDto(
	Guid Id,
	string Status,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	PlanDto Plan,
	IReadOnlyCollection<SubscriptionUsageDto> SubscriptionUsages,
	UserBasicDto? User = null,
	PaymentBasicDto? Payment = null
);

