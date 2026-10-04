using Entity = Rakushu.Domain.Entities.Linguistic.UniversalPartOfSpeech.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;

public sealed record UniversalPartOfSpeechDto(
	string Code,
	string Name,
	string VietnameseName,
	string? Description)
{
	public static UniversalPartOfSpeechDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.VietnameseName,
			e.Description);
}
