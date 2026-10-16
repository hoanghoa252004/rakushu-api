namespace Rakushu.Application.Usecases.Knowledge.JapanesePartOfSpeech.Common;

public sealed class JapanesePartOfSpeechDto
{
	public Guid Id { get; init; }
	public string Code { get; init; } = null!;
	public string Name { get; init; } = null!;
	public string JapaneseName { get; init; } = null!;
	public string? Description { get; init; }
}
