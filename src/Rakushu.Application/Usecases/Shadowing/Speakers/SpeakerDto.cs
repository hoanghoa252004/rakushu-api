using Rakushu.Application.Abstractions.Infrastructure.Storage;
using Rakushu.Domain.Entities.Shadowing.Speaker;

namespace Rakushu.Application.Usecases.Shadowing.Speakers;

public sealed record SpeakerDto(
	Guid Id,
	string Code,
	string Name,
	string JapaneseName,
	string? AvatarKey,
	string? AvatarUrl,
	string? Description,
	string ThemeColor,
	DateTimeOffset CreatedAt,
	DateTimeOffset UpdatedAt)
{
	public static async Task<SpeakerDto> FromEntityAsync(
		Speaker speaker,
		IStorageService storageService,
		CancellationToken cancellationToken)
	{
		string? url = null;
		if (!string.IsNullOrWhiteSpace(speaker.AvatarKey))
			url = await storageService.CreatePresignedReadUrlAsync(speaker.AvatarKey, cancellationToken);

		return new SpeakerDto(
			speaker.Id.Value,
			speaker.Code,
			speaker.Name,
			speaker.JapaneseName,
			speaker.AvatarKey,
			url,
			speaker.Description,
			speaker.ThemeColor,
			speaker.CreatedAt,
			speaker.UpdatedAt);
	}
}
