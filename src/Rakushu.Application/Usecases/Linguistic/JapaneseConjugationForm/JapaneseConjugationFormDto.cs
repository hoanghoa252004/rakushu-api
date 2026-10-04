using Entity = Rakushu.Domain.Entities.Linguistic.JapaneseConjugationForm.JapaneseConjugationForm;

namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm;

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
