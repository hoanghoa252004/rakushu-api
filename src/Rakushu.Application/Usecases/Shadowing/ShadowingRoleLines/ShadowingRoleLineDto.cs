using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Application.Usecases.Shadowing.ShadowingRoleLines;

public sealed record ShadowingRoleLineDto(
	Guid Id,
	Guid ShadowingRoleId,
	int SortOrder,
	string Text,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string AudioKey,
	string? AudioUrl)
{
	public static async Task<ShadowingRoleLineDto> FromEntityAsync(
		ShadowingRoleLine l,
		IStorageService storageService,
		CancellationToken cancellationToken)
	{
		string? url = null;
		if (!string.IsNullOrWhiteSpace(l.AudioKey))
			url = await storageService.CreatePresignedReadUrlAsync(l.AudioKey, cancellationToken);
		return new ShadowingRoleLineDto(
			l.Id.Value,
			l.ShadowingRoleId.Value,
			l.SortOrder,
			l.Text,
			l.StartTime,
			l.EndTime,
			l.AudioKey,
			url);
	}
}
