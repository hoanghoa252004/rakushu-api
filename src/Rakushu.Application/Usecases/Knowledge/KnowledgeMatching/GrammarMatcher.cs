using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;

public sealed class GrammarMatcher : KnowledgeMatcherBase
{
	public override bool CanHandle(KnowledgeUnitType knowledgeType)
	{
		return knowledgeType == KnowledgeUnitType.Grammar;
	}
}