using Rakushu.Domain.Entities.Knowledge.KnowledgeUnitElement;
using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;

public static class TokenSequenceMatcher
{
	private const double MinimumMatchRate = 0.8;

	// Grammar được phép có token trung gian, nhưng span không được
	// vượt quá tỷ lệ này so với số element.
	private const double GrammarMaxSpanMultiplier = 2.5;

	// Grammar luôn phải match toàn bộ element.
	// Ví dụ 4 element có thể nằm trong span tối đa 10 token.
	private const int GrammarMinimumMaxSpan = 5;

	public static bool TryMatchByLemma(
		IReadOnlyList<Token> tokens,
		IReadOnlyList<KnowledgeUnitElement> elements,
		out IReadOnlyList<Token> matchedTokens,
		out double matchRate,
		bool requireExactSequence = false)
	{
		matchedTokens = Array.Empty<Token>();
		matchRate = 0;

		if (tokens.Count == 0 || elements.Count == 0)
		{
			return false;
		}

		var orderedTokens = tokens
			.OrderBy(x => x.Sequence)
			.ToList();

		var orderedElements = elements
			.OrderBy(x => x.Sequence)
			.ToList();

		if (orderedElements.Any(
				x => string.IsNullOrWhiteSpace(x.Surface) &&
					 string.IsNullOrWhiteSpace(x.Lemma)))
		{
			return false;
		}

		return requireExactSequence
			? TryMatchGrammar(
				orderedTokens,
				orderedElements,
				out matchedTokens,
				out matchRate)
			: TryMatchFlexible(
				orderedTokens,
				orderedElements,
				out matchedTokens,
				out matchRate);
	}

	private static bool TryMatchGrammar(
		IReadOnlyList<Token> tokens,
		IReadOnlyList<KnowledgeUnitElement> elements,
		out IReadOnlyList<Token> matchedTokens,
		out double matchRate)
	{
		matchedTokens = Array.Empty<Token>();
		matchRate = 0;

		var maxSpanLength = Math.Max(
			GrammarMinimumMaxSpan,
			(int)Math.Ceiling(
				elements.Count * GrammarMaxSpanMultiplier));

		List<Token>? bestMatch = null;
		var bestSpanLength = int.MaxValue;

		// Thử từng token làm điểm bắt đầu.
		for (var start = 0; start < tokens.Count; start++)
		{
			var currentMatches = new List<Token>();
			var elementIndex = 0;

			for (var tokenIndex = start;
				 tokenIndex < tokens.Count;
				 tokenIndex++)
			{
				var token = tokens[tokenIndex];

				var spanLength =
					token.Sequence - tokens[start].Sequence + 1;

				// Không cho grammar match các token quá xa nhau.
				if (spanLength > maxSpanLength)
				{
					break;
				}

				var element = elements[elementIndex];

				if (!IsTokenMatch(token, element))
				{
					continue;
				}

				currentMatches.Add(token);
				elementIndex++;

				// Đã match đủ toàn bộ grammar.
				if (elementIndex == elements.Count)
				{
					var matchedSpanLength =
						currentMatches[^1].Sequence -
						currentMatches[0].Sequence + 1;

					if (matchedSpanLength < bestSpanLength)
					{
						bestMatch = currentMatches;
						bestSpanLength = matchedSpanLength;
					}

					break;
				}
			}
		}

		if (bestMatch is null)
		{
			return false;
		}

		matchedTokens = bestMatch;
		matchRate = 1.0;

		return true;
	}

	private static bool TryMatchFlexible(
		IReadOnlyList<Token> tokens,
		IReadOnlyList<KnowledgeUnitElement> elements,
		out IReadOnlyList<Token> matchedTokens,
		out double matchRate)
	{
		matchedTokens = Array.Empty<Token>();
		matchRate = 0;

		var validElements = elements
			.Where(x =>
				!string.IsNullOrWhiteSpace(x.Surface) ||
				!string.IsNullOrWhiteSpace(x.Lemma))
			.ToList();

		if (validElements.Count == 0)
		{
			return false;
		}

		var requiredMatches = (int)Math.Ceiling(
			validElements.Count * MinimumMatchRate);

		List<Token>? bestMatchedTokens = null;
		var bestMatchCount = 0;
		var bestSpanLength = int.MaxValue;

		for (var start = 0; start < tokens.Count; start++)
		{
			var currentMatches = new List<Token>();
			var elementIndex = 0;

			for (var tokenIndex = start;
				 tokenIndex < tokens.Count &&
				 elementIndex < validElements.Count;
				 tokenIndex++)
			{
				var token = tokens[tokenIndex];
				var element = validElements[elementIndex];

				if (!IsTokenMatch(token, element))
				{
					continue;
				}

				currentMatches.Add(token);
				elementIndex++;
			}

			if (currentMatches.Count < requiredMatches)
			{
				continue;
			}

			var spanLength =
				currentMatches[^1].Sequence -
				currentMatches[0].Sequence + 1;

			if (bestMatchedTokens is null ||
				currentMatches.Count > bestMatchCount ||
				(currentMatches.Count == bestMatchCount &&
				 spanLength < bestSpanLength))
			{
				bestMatchedTokens = currentMatches;
				bestMatchCount = currentMatches.Count;
				bestSpanLength = spanLength;
			}
		}

		if (bestMatchedTokens is null)
		{
			return false;
		}

		matchedTokens = bestMatchedTokens;
		matchRate = (double)bestMatchCount / validElements.Count;

		return true;
	}

	private static bool IsTokenMatch(
		Token token,
		KnowledgeUnitElement element)
	{
		// Ưu tiên Surface.
		if (!string.IsNullOrWhiteSpace(token.Surface) &&
			!string.IsNullOrWhiteSpace(element.Surface) &&
			string.Equals(
				token.Surface,
				element.Surface,
				StringComparison.Ordinal))
		{
			return true;
		}

		// Nếu Surface không khớp, thử Lemma.
		return !string.IsNullOrWhiteSpace(token.Lemma) &&
			   !string.IsNullOrWhiteSpace(element.Lemma) &&
			   string.Equals(
				   token.Lemma,
				   element.Lemma,
				   StringComparison.Ordinal);
	}
}