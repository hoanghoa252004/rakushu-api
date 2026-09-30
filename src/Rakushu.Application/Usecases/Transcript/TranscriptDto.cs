using Entity = Rakushu.Domain.Entities.Video.Transcript.Transcript;

using Rakushu.Domain.Entities.Video.Transcript;

namespace Rakushu.Application.Usecases.Transcript;

public sealed record TranscriptDto(
	Guid VideoId,
	string FullText)
{
	public static TranscriptDto FromEntity(Entity e) =>
		new(
			e.VideoId.Value,
			e.FullText);
}
