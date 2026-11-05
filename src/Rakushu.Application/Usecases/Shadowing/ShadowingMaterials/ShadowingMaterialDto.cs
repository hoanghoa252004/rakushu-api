using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Application.Common.Pagination;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines;
using Rakushu.Application.Usecases.Shadowing.ShadowingRoles;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRole;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingMaterials;

public sealed record ShadowingMaterialDto(
	Guid Id,
	string Type,
	string Status,
	Guid? ShadowingMaterialRequestId,
	Guid VideoId,
	int Version,
	DateTimeOffset CreatedAt,
	IReadOnlyList<ShadowingRoleDto>? Roles = null,
	PaginatedList<ShadowingRoleLineDto>? RoleLines = null)
{
	// Roles (with Speaker) and paged role lines are included only for Reviewing / Ready materials.
	public static async Task<ShadowingMaterialDto> FromEntityWithDetailsAsync(
		ShadowingMaterial m,
		IShadowingRoleRepository roleRepository,
		IShadowingRoleLineRepository roleLineRepository,
		IStorageService storageService,
		int linePageNumber,
		int linePageSize,
		CancellationToken cancellationToken)
	{
		var dto = FromEntity(m);
		if (m.Status != ShadowingMaterialStatus.Reviewing && m.Status != ShadowingMaterialStatus.Ready)
			return dto;

		var roles = await roleRepository.GetByMaterialIdAsync(m.Id, cancellationToken);
		var roleDtos = new List<ShadowingRoleDto>();
		foreach (var r in roles)
			roleDtos.Add(await ShadowingRoleDto.FromEntityAsync(r, storageService, cancellationToken));

		var (lines, total) = await roleLineRepository.GetPagedAsync(
			m.Id, null, null, linePageNumber, linePageSize, cancellationToken);
		var lineDtos = new List<ShadowingRoleLineDto>();
		foreach (var l in lines)
			lineDtos.Add(await ShadowingRoleLineDto.FromEntityAsync(l, storageService, cancellationToken));

		return dto with
		{
			Roles = roleDtos,
			RoleLines = PaginatedList<ShadowingRoleLineDto>.Create(lineDtos, total, linePageNumber, linePageSize)
		};
	}

	public static ShadowingMaterialDto FromEntity(ShadowingMaterial m) => new(
		m.Id.Value,
		m.Type.ToString(),
		m.Status.ToString(),
		m.ShadowingMaterialRequestId?.Value,
		m.VideoId.Value,
		m.Version,
		m.CreatedAt);
}
