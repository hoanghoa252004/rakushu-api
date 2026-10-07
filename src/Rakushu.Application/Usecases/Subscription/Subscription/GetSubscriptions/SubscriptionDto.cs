using Rakushu.Application.Usecases.Payment.Payment.GetPaymentById;
using Rakushu.Application.Usecases.Subscription.Plan.GetPlanById;

namespace Rakushu.Application.Usecases.Subscription.Subscription.GetSubscriptions;

public sealed record UserBasicDto(
	Guid Id,
	string FullName,
	string Email
);

public sealed record SubscriptionDto(
	Guid Id,
	string Status,
	DateTimeOffset StartAt,
	DateTimeOffset EndAt,
	PaymentBasicDto? Payment,
	UserBasicDto User,
	PlanBasicDto Plan
);

