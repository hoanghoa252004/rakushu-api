using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;
public sealed class CollocationMatcher : KnowledgeMatcherBase
{
	public override bool CanHandle(KnowledgeUnitType knowledgeType)
	{
		return knowledgeType == KnowledgeUnitType.Collocation;
	}
}