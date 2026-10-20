using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;

public interface IKnowledgeMatcher
{
	bool CanHandle(KnowledgeUnitType knowledgeType);

	IReadOnlyList<KnowledgeMatchCandidate> Match(
		KnowledgeUnit.KnowledgeUnit knowledgeUnit,
		Guid transcriptSegmentId,
		IReadOnlyList<Token> tokens);
}

public sealed record KnowledgeMatchCandidate(
	Guid KnowledgeUnitId,
	Guid transcriptSegmentId,
	int StartTokenSequence,
	int EndTokenSequence,
	int StartIndex,
	int EndIndex,
	string MatchedText);

public sealed record KnowledgeCollectionPriority(
	Guid KnowledgeCollectionId,
	int Priority);

public sealed record TranscriptSegmentForMatching(
	Guid Id,
	Guid ContentId,
	string Text);