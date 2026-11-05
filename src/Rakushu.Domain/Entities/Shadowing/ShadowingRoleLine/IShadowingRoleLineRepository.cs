using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public interface IShadowingRoleLineRepository : IBaseRepository<ShadowingRoleLine, ShadowingRoleLineId>
{
	Task<bool> SortOrderExistsAsync(
		ShadowingMaterialId materialId,
		int sortOrder,
		ShadowingRoleLineId? excludeId,
		CancellationToken cancellationToken = default);

	Task<int> GetNextSortOrderAsync(
		ShadowingMaterialId materialId,
		CancellationToken cancellationToken = default);

	Task<(IReadOnlyList<ShadowingRoleLine> Items, int TotalCount)> GetPagedAsync(
		ShadowingMaterialId materialId,
		SpeakerId? speakerId,
		ShadowingRoleId? roleId,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default);
}
