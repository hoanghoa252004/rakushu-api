using Rakushu.Domain.Entities.Shadowing.ShadowingMaterialRequest;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterialRequests;

public sealed record ShadowingMaterialRequestDto(
	Guid Id,
	string Code,
	Guid VideoId,
	Guid LearnerId,
	string Status,
	string? RejectReason,
	DateTimeOffset RequestedAt,
	DateTimeOffset UpdatedAt,
	DateTimeOffset? CompletedAt)
{
	public static ShadowingMaterialRequestDto FromEntity(ShadowingMaterialRequest r) => new(
		r.Id.Value,
		r.Code,
		r.VideoId.Value,
		r.LearnerId.Value,
		r.Status.ToString(),
		r.RejectReason,
		r.RequestedAt,
		r.UpdatedAt,
		r.CompletedAt);
}
