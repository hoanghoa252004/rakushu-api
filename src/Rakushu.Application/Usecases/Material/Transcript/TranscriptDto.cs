using Entity = Rakushu.Domain.Entities.Material.Transcript.Transcript;

namespace Rakushu.Application.Usecases.Material.Transcript;

public sealed record TranscriptDto(
	Guid VideoId,
	string FullText)
{
	public static TranscriptDto FromEntity(Entity e) =>
		new(
			e.VideoId.Value,
			e.FullText);
}
