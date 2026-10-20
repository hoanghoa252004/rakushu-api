
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class KnowledgeMatcherBase : IKnowledgeMatcher
{
	public abstract bool CanHandle(
		KnowledgeUnitType knowledgeType);

	public IReadOnlyList<KnowledgeMatchCandidate> Match(
		KnowledgeUnit knowledgeUnit,
		Guid transcriptSegmentId,
		IReadOnlyList<Token> tokens)
	{
		if (knowledgeUnit.Elements.Count == 0 ||
			tokens.Count == 0)
		{
			return Array.Empty<KnowledgeMatchCandidate>();
		}

		var isGrammar =
			knowledgeUnit.KnowledgeType == KnowledgeUnitType.Grammar;

		if (!TokenSequenceMatcher.TryMatchByLemma(
				tokens,
				knowledgeUnit.Elements.ToList(),
				out var matchedTokens,
				out _,
				requireExactSequence: isGrammar))
		{
			return Array.Empty<KnowledgeMatchCandidate>();
		}

		var candidateTokens = isGrammar
			? matchedTokens
			: JapaneseVerbSpanExpander.Expand(
				tokens,
				matchedTokens);

		if (candidateTokens.Count == 0)
		{
			return Array.Empty<KnowledgeMatchCandidate>();
		}

		var orderedCandidateTokens = candidateTokens
			.OrderBy(x => x.Sequence)
			.ToList();

		var firstToken = orderedCandidateTokens.First();
		var lastToken = orderedCandidateTokens.Last();

		var startIndex = firstToken.StartIndex;
		var endIndex = lastToken.EndIndex;

		var matchedText = string.Concat(
			tokens
				.Where(x =>
					x.StartIndex >= startIndex &&
					x.EndIndex <= endIndex)
				.OrderBy(x => x.StartIndex)
				.Select(x => x.Surface));

		var candidate = new KnowledgeMatchCandidate(
			knowledgeUnit.Id.Value,
			transcriptSegmentId,
			firstToken.Sequence,
			lastToken.Sequence,
			startIndex,
			endIndex,
			matchedText);

		return new[] { candidate };
	}
}
