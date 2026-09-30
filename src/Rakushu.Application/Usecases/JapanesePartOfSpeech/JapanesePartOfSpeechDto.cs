using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech.JapanesePartOfSpeech;

using Rakushu.Domain.Entities.LinguisticMetadata.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.JapanesePartOfSpeech;

public sealed record JapanesePartOfSpeechDto(
	string Code,
	string Name,
	string VietnameseName,
	string? Description)
{
	public static JapanesePartOfSpeechDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.VietnameseName,
			e.Description);
}
