using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Bunsetsu;

using Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

namespace Rakushu.Application.Usecases.Bunsetsu;

public sealed record BunsetsuDto(
	Guid TranscriptSegmentId,
	string Text,
	int StartIndex,
	int EndIndex,
	int Sequence)
{
	public static BunsetsuDto FromEntity(Entity e) =>
		new(
			e.TranscriptSegmentId.Value,
			e.Text,
			e.StartIndex,
			e.EndIndex,
			e.Sequence);
}
