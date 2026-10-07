using Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;

namespace Rakushu.Application.Usecases.Subscription.Entitlement.GetEntitlements;

public sealed record EntitlementDto(
	Guid Id,
	bool IsEnabled,
	string LimitUnit,
	int LimitValue,
	string LimitPeriod,
	FeatureDto Feature
);
