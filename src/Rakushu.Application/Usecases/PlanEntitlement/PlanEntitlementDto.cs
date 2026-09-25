namespace Rakushu.Application.Usecases.PlanEntitlement;

public sealed record PlanEntitlementDto(
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
