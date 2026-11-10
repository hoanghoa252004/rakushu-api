using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingSession;

public sealed record ShadowingSessionFilter(
	UserId? MemberId = null,
	UserId? HostId = null,
	ShadowingSessionStatus? Status = null,
	ShadowingSessionVisibility? Visibility = null,
	ShadowingMaterialId? MaterialId = null,
	DateTimeOffset? StartsAfter = null);
