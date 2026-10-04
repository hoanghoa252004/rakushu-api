namespace Rakushu.Application.Usecases.Subscription.Entitlement;

public sealed record EntitlementDto(
	Guid Id,
	Guid PlanId,
	Guid FeatureId,
	string FeatureCode,
	string FeatureName,
	bool IsEnabled,
	int LimitValue,
	string LimitUnit,
	string LimitPeriod
);
