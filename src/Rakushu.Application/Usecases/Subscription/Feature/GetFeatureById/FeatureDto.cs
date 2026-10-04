namespace Rakushu.Application.Usecases.Subscription.Feature.GetFeatureById;

public sealed record FeatureDto(
	Guid Id,
	string Code,
	string Name,
	string Status,
	DateTime CreatedAt,
	DateTime UpdatedAt,
	string? Description = null
);
