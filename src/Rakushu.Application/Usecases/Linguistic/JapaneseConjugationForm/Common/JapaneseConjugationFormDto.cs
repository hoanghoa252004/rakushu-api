namespace Rakushu.Application.Usecases.Linguistic.JapaneseConjugationForm.Common;

public sealed class JapaneseConjugationFormDto
{
	public Guid Id { get; init; }
	public string Code { get; init; } = null!;
	public string Name { get; init; } = null!;
	public string JapaneseName { get; init; } = null!;
	public string? Description { get; init; }
}
