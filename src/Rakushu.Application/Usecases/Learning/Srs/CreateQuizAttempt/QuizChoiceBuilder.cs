using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Learning.Srs.Common;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed record QuizChoiceBuildResult(
	string Direction,
	string Prompt,
	string CorrectAnswerText,
	IReadOnlyList<QuizChoiceDto> Choices,
	string CorrectChoiceId,
	string QuestionType);

/// <summary>
/// Builds deterministic, backend-owned MCQ choices from authoritative vocabulary data.
/// Each question has 1 correct answer and 3 random distractors from knowledge meanings.
/// </summary>
internal static class QuizChoiceBuilder
{
	public const string JapaneseToVietnamese = "JapaneseToVietnamese";
	public const string VietnameseToJapanese = "VietnameseToJapanese";
	public const string JapaneseToVietnameseQuestionType = "WORD_TO_MEANING_MCQ";
	public const string VietnameseToJapaneseQuestionType = "MEANING_TO_WORD_MCQ";

	public static bool TryBuild(
		string direction,
		string expression,
		string meaning,
		string? reading,
		IReadOnlyList<QuizVocabularyEntry> vocabularyEntries,
		out QuizChoiceBuildResult? result,
		out string failureReason)
	{
		result = null;
		failureReason = string.Empty;

		var authoritativeMeaning = meaning?.Trim() ?? string.Empty;
		var authoritativeExpression = expression?.Trim() ?? string.Empty;

		if (string.IsNullOrWhiteSpace(authoritativeMeaning))
		{
			failureReason = "The authoritative meaning is empty.";
			return false;
		}

		if (string.IsNullOrWhiteSpace(authoritativeExpression))
		{
			failureReason = "The authoritative expression is empty.";
			return false;
		}

		string prompt;
		string correctAnswerText;
		string questionType;
		List<string> distractors;

		if (direction == VietnameseToJapanese)
		{
			questionType = VietnameseToJapaneseQuestionType;
			prompt = $"Từ tiếng Nhật nào có nghĩa là \"{authoritativeMeaning}\"?";
			correctAnswerText = authoritativeExpression;

			var candidateDistractors = vocabularyEntries
				.Where(entry => !string.IsNullOrWhiteSpace(entry.Expression))
				.Where(entry => !string.Equals(entry.Expression.Trim(), authoritativeExpression, StringComparison.OrdinalIgnoreCase))
				.Where(entry => !string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
				.Select(entry => entry.Expression.Trim())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (candidateDistractors.Count < 3)
			{
				failureReason = $"Only {candidateDistractors.Count} distinct Japanese distractors are available in knowledge base; 3 are required.";
				return false;
			}

			distractors = candidateDistractors
				.OrderBy(_ => Random.Shared.Next())
				.Take(3)
				.ToList();
		}
		else
		{
			direction = JapaneseToVietnamese;
			questionType = JapaneseToVietnameseQuestionType;
			prompt = $"Từ tiếng Nhật \"{authoritativeExpression}\" có nghĩa là gì trong tiếng Việt?";
			correctAnswerText = authoritativeMeaning;

			var candidateDistractors = vocabularyEntries
				.Where(entry => !string.IsNullOrWhiteSpace(entry.Meaning))
				.Where(entry => !string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
				.Select(entry => entry.Meaning.Trim())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (candidateDistractors.Count < 3)
			{
				failureReason = $"Only {candidateDistractors.Count} distinct Vietnamese distractors are available in knowledge base; 3 are required.";
				return false;
			}

			distractors = candidateDistractors
				.OrderBy(_ => Random.Shared.Next())
				.Take(3)
				.ToList();
		}

		// Combine 3 distractors and 1 correct answer, then shuffle
		var allChoices = distractors.Concat([correctAnswerText]).ToList();
		for (var index = allChoices.Count - 1; index > 0; index--)
		{
			var swapIndex = Random.Shared.Next(index + 1);
			(allChoices[index], allChoices[swapIndex]) = (allChoices[swapIndex], allChoices[index]);
		}

		var choices = allChoices
			.Select((text, index) => new QuizChoiceDto(((char)('A' + index)).ToString(), text))
			.ToList();

		var correctChoice = choices.FirstOrDefault(c =>
			string.Equals(c.Text, correctAnswerText, StringComparison.OrdinalIgnoreCase));

		if (correctChoice is null)
		{
			failureReason = "Could not locate correct choice after shuffle.";
			return false;
		}

		result = new QuizChoiceBuildResult(
			direction,
			prompt,
			correctAnswerText,
			choices,
			correctChoice.Id,
			questionType);

		return true;
	}
}
