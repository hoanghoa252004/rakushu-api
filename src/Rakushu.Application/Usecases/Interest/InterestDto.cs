using Entity = Rakushu.Domain.Entities.User.Profile.Interest.Interest;

using Rakushu.Domain.Entities.User.Profile.Interest;

namespace Rakushu.Application.Usecases.Interest;

public sealed record InterestDto(
	Guid ProfileId,
	Guid ContentCategoryId,
	int Priority)
{
	public static InterestDto FromEntity(Entity e) =>
		new(
			e.ProfileId.Value,
			e.ContentCategoryId.Value,
			e.Priority);
}
