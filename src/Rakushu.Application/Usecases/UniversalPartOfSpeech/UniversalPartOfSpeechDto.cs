using Entity = Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech.UniversalPartOfSpeech;

using Rakushu.Domain.Entities.LinguisticMetadata.UniversalPartOfSpeech;

namespace Rakushu.Application.Usecases.UniversalPartOfSpeech;

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
