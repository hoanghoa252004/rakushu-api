using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Usecases.Shadowing.Speakers;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoles;

public sealed record ShadowingRoleDto(
	Guid Id,
	Guid ShadowingMaterialId,
	Guid SpeakerId,
	int SortOrder,
	SpeakerDto? Speaker = null)
{
	public static ShadowingRoleDto FromEntity(ShadowingRole r) =>
		new(r.Id.Value, r.ShadowingMaterialId.Value, r.SpeakerId.Value, r.SortOrder);

	public static async Task<ShadowingRoleDto> FromEntityAsync(
		ShadowingRole r,
		IStorageService storageService,
		CancellationToken cancellationToken)
	{
		var speaker = r.Speaker is null
			? null
			: await SpeakerDto.FromEntityAsync(r.Speaker, storageService, cancellationToken);

		return new ShadowingRoleDto(r.Id.Value, r.ShadowingMaterialId.Value, r.SpeakerId.Value, r.SortOrder, speaker);
	}
}
