namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;

public sealed record FeatureDto(
	Guid Id,
	string Code,
	string Name,
	bool IsActive,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt,
	string? Description = null
);
