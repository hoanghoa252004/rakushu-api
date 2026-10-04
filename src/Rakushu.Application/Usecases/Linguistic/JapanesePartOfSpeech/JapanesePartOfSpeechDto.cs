using Entity = Rakushu.Domain.Entities.Linguistic.JapanesePartOfSpeech.JapanesePartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.JapanesePartOfSpeech;

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
