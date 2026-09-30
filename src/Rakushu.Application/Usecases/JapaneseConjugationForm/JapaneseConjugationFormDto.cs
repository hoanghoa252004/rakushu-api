using Entity = Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm.JapaneseConjugationForm;

using Rakushu.Domain.Entities.LinguisticMetadata.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.JapaneseConjugationForm;

public sealed record JapaneseConjugationFormDto(
	string Code,
	string Name,
	string VietnameseName,
	string? Description)
{
	public static JapaneseConjugationFormDto FromEntity(Entity e) =>
		new(
			e.Code,
			e.Name,
			e.VietnameseName,
			e.Description);
}
