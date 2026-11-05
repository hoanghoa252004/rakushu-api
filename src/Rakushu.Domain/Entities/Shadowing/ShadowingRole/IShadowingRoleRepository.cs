using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRole;

public interface IShadowingRoleRepository : IBaseRepository<ShadowingRole, ShadowingRoleId>
{
	Task<IReadOnlyList<ShadowingRole>> GetByMaterialIdAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default);

	Task<bool> SpeakerExistsAsync(
		ShadowingMaterialId materialId,
		SpeakerId speakerId,
		ShadowingRoleId? excludeId,
		CancellationToken cancellationToken = default);

	Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleId? excludeId,
		CancellationToken cancellationToken = default);

	Task<bool> VideoHasSpeakerAsync(
		VideoId videoId,
		SpeakerId speakerId,
		CancellationToken cancellationToken = default);

	Task<bool> HasRoleWithoutLinesAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default);
}
