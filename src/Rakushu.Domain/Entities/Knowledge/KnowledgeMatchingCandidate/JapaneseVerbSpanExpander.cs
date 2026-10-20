using Rakushu.Domain.Entities.Material.Token;
using System;
using System.Collections.Generic;
using System.Linq;

public static class JapaneseVerbSpanExpander
{
	private static readonly HashSet<string> AuxiliaryLemmas =
		new(StringComparer.Ordinal)
		{
			"れる",
			"られる",
			"せる",
			"させる",
			"ない",
			"ぬ",
			"た",
			"ます",
			"です",
			"だ",
			"う",
			"よう",
			"たい",
			"たがる",
			"そうだ",
			"ようだ",
			"らしい",
			"みたいだ",
			"しまう",
			"おく",
			"ある",
			"いる",
			"くる",
			"いく"
		};

	private static readonly HashSet<string> ConjunctiveSurfaces =
		new(StringComparer.Ordinal)
		{
			"て",
			"で"
		};

	public static IReadOnlyList<Token> Expand(
		IReadOnlyList<Token> tokens,
		IReadOnlyList<Token> matchedTokens)
	{
		if (tokens.Count == 0 || matchedTokens.Count == 0)
		{
			return Array.Empty<Token>();
		}

		var orderedTokens = tokens
			.OrderBy(x => x.Sequence)
			.ToList();

		var firstMatched = matchedTokens
			.OrderBy(x => x.Sequence)
			.First();

		var lastMatched = matchedTokens
			.OrderBy(x => x.Sequence)
			.Last();

		var startPosition = orderedTokens.FindIndex(
			x => x.Sequence == firstMatched.Sequence);

		var endPosition = orderedTokens.FindIndex(
			x => x.Sequence == lastMatched.Sequence);

		if (startPosition < 0 || endPosition < 0)
		{
			return matchedTokens;
		}

		// Chỉ mở rộng khi token cuối đang match là một phần
		// của chuỗi động từ, hoặc bản thân nó là động từ.
		if (!IsVerbLike(lastMatched))
		{
			return GetContinuousSpan(
				orderedTokens,
				startPosition,
				endPosition);
		}

		while (endPosition + 1 < orderedTokens.Count)
		{
			var nextToken = orderedTokens[endPosition + 1];

			if (!ShouldInclude(nextToken))
			{
				break;
			}

			endPosition++;
		}

		return GetContinuousSpan(
			orderedTokens,
			startPosition,
			endPosition);
	}

	private static bool ShouldInclude(Token token)
	{
		if (string.IsNullOrWhiteSpace(token.Surface))
		{
			return false;
		}

		// Ví dụ:
		// 検出 + さ + れ + た
		// 分析 + し + て + いる
		// 食べ + られ + ない
		if (AuxiliaryLemmas.Contains(token.Lemma ?? string.Empty))
		{
			return true;
		}

		// Các token nối tiếp của chuỗi te-form.
		if (ConjunctiveSurfaces.Contains(token.Surface))
		{
			return true;
		}

		return false;
	}

	private static bool IsVerbLike(Token token)
	{
		var lemma = token.Lemma ?? string.Empty;

		// Đây là kiểm tra sơ bộ. Trong implementation thực tế,
		// nên xác định động từ dựa trên JapanesePartOfSpeech.
		return lemma.EndsWith("る", StringComparison.Ordinal) ||
			   lemma.EndsWith("う", StringComparison.Ordinal) ||
			   lemma.EndsWith("く", StringComparison.Ordinal) ||
			   lemma.EndsWith("ぐ", StringComparison.Ordinal) ||
			   lemma.EndsWith("す", StringComparison.Ordinal) ||
			   lemma.EndsWith("つ", StringComparison.Ordinal) ||
			   lemma.EndsWith("ぬ", StringComparison.Ordinal) ||
			   lemma.EndsWith("ぶ", StringComparison.Ordinal) ||
			   lemma.EndsWith("む", StringComparison.Ordinal) ||
			   lemma.EndsWith("る", StringComparison.Ordinal) ||
			   lemma == "する" ||
			   lemma == "来る";
	}

	private static IReadOnlyList<Token> GetContinuousSpan(
		IReadOnlyList<Token> orderedTokens,
		int startPosition,
		int endPosition)
	{
		return orderedTokens
			.Skip(startPosition)
			.Take(endPosition - startPosition + 1)
			.ToList();
	}
}