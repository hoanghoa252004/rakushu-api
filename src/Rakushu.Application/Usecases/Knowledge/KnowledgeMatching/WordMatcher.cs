using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;


public sealed class WordMatcher : KnowledgeMatcherBase
{
	public override bool CanHandle(KnowledgeUnitType knowledgeType)
	{
		return knowledgeType is
			KnowledgeUnitType.Word or
			KnowledgeUnitType.Terminology or
			KnowledgeUnitType.ProperNoun;
	}
}