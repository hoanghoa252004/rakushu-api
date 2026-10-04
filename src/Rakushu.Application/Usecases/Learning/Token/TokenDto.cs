using Entity = Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token.Token;

using Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

namespace Rakushu.Application.Usecases.Learning.Token;

public sealed record TokenDto(
	Guid BunsetsuId,
	string Surface,
	string Lemma,
	string Reading,
	Guid JapanesePartOfSpeechId,
	Guid UniversalPartOfSpeechId,
	Guid DependencyRelationshipId,
	int StartIndex,
	int EndIndex,
	int Sequence)
{
	public static TokenDto FromEntity(Entity e) =>
		new(
			e.BunsetsuId.Value,
			e.Surface,
			e.Lemma,
			e.Reading,
			e.JapanesePartOfSpeechId.Value,
			e.UniversalPartOfSpeechId.Value,
			e.DependencyRelationshipId.Value,
			e.StartIndex,
			e.EndIndex,
			e.Sequence);
}
