using Microsoft.Extensions.Logging;
using Rakushu.Domain.Entities.Knowledge.KnowledgeMatchingCandidate;
using Rakushu.Domain.Entities.Knowledge.KnowledgeUnit;
using Rakushu.Domain.Entities.Material.Token;

public sealed class KnowledgeMatchingService
{
	private readonly IReadOnlyList<IKnowledgeMatcher> _matchers;
	private readonly ILogger<KnowledgeMatchingService> _logger;

	public KnowledgeMatchingService(
		IEnumerable<IKnowledgeMatcher> matchers,
		ILogger<KnowledgeMatchingService> logger)
	{
		_matchers = matchers.ToList();
		_logger = logger;
	}

	public IReadOnlyList<KnowledgeMatchCandidate> Match(
		IReadOnlyCollection<KnowledgeUnit> knowledgeUnits,
		Guid transcriptSegmentId,
		IReadOnlyList<Token> tokens)
	{
		using var scope = _logger.BeginScope(
			"KnowledgeMatching SegmentId={SegmentId}",
			transcriptSegmentId);

		_logger.LogInformation(
			"========== MATCHING START | SegmentId={SegmentId} | KnowledgeUnitCount={UnitCount} | TokenCount={TokenCount} ==========",
			transcriptSegmentId,
			knowledgeUnits.Count,
			tokens.Count);

		if (knowledgeUnits.Count == 0 || tokens.Count == 0)
		{
			_logger.LogWarning(
				"Matching stopped: KnowledgeUnitCount={UnitCount}, TokenCount={TokenCount}",
				knowledgeUnits.Count,
				tokens.Count);

			return Array.Empty<KnowledgeMatchCandidate>();
		}

		var orderedTokens = tokens
			.OrderBy(x => x.Sequence)
			.ToList();

		// 1. Log toàn bộ token của transcript.
		_logger.LogInformation("========== TRANSCRIPT TOKENS ==========");

		foreach (var token in orderedTokens)
		{
			_logger.LogInformation(
				"TOKEN | Seq={Sequence} | Surface=[{Surface}] | Lemma=[{Lemma}] | Start={StartIndex} | End={EndIndex}",
				token.Sequence,
				token.Surface,
				token.Lemma,
				token.StartIndex,
				token.EndIndex);
		}

		var candidates = new List<KnowledgeMatchCandidate>();

		// 2. Xử lý từng Knowledge Unit.
		foreach (var knowledgeUnit in knowledgeUnits)
		{
			_logger.LogInformation(
				"========== KNOWLEDGE UNIT | Id={Id} | Expression=[{Expression}] | Type={Type} | Active={Active} | ElementCount={ElementCount} ==========",
				knowledgeUnit.Id.Value,
				knowledgeUnit.Expression,
				knowledgeUnit.KnowledgeType,
				knowledgeUnit.IsActive,
				knowledgeUnit.Elements.Count);

			// 3. Log các element của Knowledge Unit.
			foreach (var element in knowledgeUnit.Elements.OrderBy(x => x.Sequence))
			{
				_logger.LogInformation(
					"ELEMENT | UnitId={UnitId} | Seq={Sequence} | Surface=[{Surface}] | Lemma=[{Lemma}] | Start={StartIndex} | End={EndIndex}",
					knowledgeUnit.Id.Value,
					element.Sequence,
					element.Surface,
					element.Lemma,
					element.StartIndex,
					element.EndIndex);
			}

			if (!knowledgeUnit.IsActive)
			{
				_logger.LogWarning(
					"SKIP | UnitId={UnitId} | Reason=Inactive",
					knowledgeUnit.Id.Value);

				continue;
			}

			if (knowledgeUnit.Elements.Count == 0)
			{
				_logger.LogWarning(
					"SKIP | UnitId={UnitId} | Reason=NoElements",
					knowledgeUnit.Id.Value);

				continue;
			}

			var matchingMatchers = _matchers
				.Where(x => x.CanHandle(knowledgeUnit.KnowledgeType))
				.ToList();

			if (matchingMatchers.Count == 0)
			{
				_logger.LogWarning(
					"SKIP | UnitId={UnitId} | Type={Type} | Reason=NoMatcherRegistered",
					knowledgeUnit.Id.Value,
					knowledgeUnit.KnowledgeType);

				continue;
			}

			if (matchingMatchers.Count > 1)
			{
				_logger.LogError(
					"Multiple matchers registered | UnitId={UnitId} | Type={Type} | Matchers={Matchers}",
					knowledgeUnit.Id.Value,
					knowledgeUnit.KnowledgeType,
					string.Join(", ", matchingMatchers.Select(x => x.GetType().Name)));

				throw new InvalidOperationException(
					$"Multiple matchers registered for knowledge type '{knowledgeUnit.KnowledgeType}'.");
			}

			var matcher = matchingMatchers[0];

			_logger.LogInformation(
				"MATCHER START | UnitId={UnitId} | Expression=[{Expression}] | Matcher={Matcher}",
				knowledgeUnit.Id.Value,
				knowledgeUnit.Expression,
				matcher.GetType().Name);

			var matches = matcher.Match(
				knowledgeUnit,
				transcriptSegmentId,
				orderedTokens);

			_logger.LogInformation(
				"MATCHER END | UnitId={UnitId} | Expression=[{Expression}] | Matcher={Matcher} | MatchCount={MatchCount}",
				knowledgeUnit.Id.Value,
				knowledgeUnit.Expression,
				matcher.GetType().Name,
				matches.Count);

			foreach (var match in matches)
			{
				_logger.LogInformation(
					"CANDIDATE | UnitId={UnitId} | Expression=[{Expression}] | StartSeq={StartSeq} | EndSeq={EndSeq} | StartIndex={StartIndex} | EndIndex={EndIndex} | MatchedText=[{MatchedText}]",
					match.KnowledgeUnitId,
					knowledgeUnit.Expression,
					match.StartTokenSequence,
					match.EndTokenSequence,
					match.StartIndex,
					match.EndIndex,
					match.MatchedText);
			}

			candidates.AddRange(matches);
		}

		_logger.LogInformation(
			"========== MATCHING FINISHED | SegmentId={SegmentId} | CandidateCount={CandidateCount} ==========",
			transcriptSegmentId,
			candidates.Count);

		return candidates;
	}
}