namespace Rakushu.Application.Usecases.Linguistic.UniversalPartOfSpeech;

public sealed record UniversalPartOfSpeechDto(
	Guid Id,
	string Code,
	string Name,
	string VietnameseName,
	string? Description
	);