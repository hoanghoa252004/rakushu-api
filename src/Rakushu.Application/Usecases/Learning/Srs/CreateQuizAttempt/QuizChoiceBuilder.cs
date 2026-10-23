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
/// This type deliberately does not query persistence or invoke the LLM.
/// </summary>
internal static class QuizChoiceBuilder
{
    private const string JapaneseToVietnamese = "JapaneseToVietnamese";
    private const string VietnameseToJapanese = "VietnameseToJapanese";
    private const string JapaneseToVietnameseQuestionType = "WORD_TO_MEANING_MCQ";
    private const string VietnameseToJapaneseQuestionType = "MEANING_TO_WORD_MCQ";

    public static bool TryBuild(
        string direction,
        string expression,
        string meaning,
        string? reading,
        IReadOnlyList<QuizVocabularyEntry> vocabularyEntries,
        IReadOnlySet<string> allCorrectMeanings,
        IReadOnlySet<string> allCorrectExpressions,
        string prompt,
        out QuizChoiceBuildResult? result,
        out string failureReason)
    {
        result = null;
        failureReason = string.Empty;
        var authoritativeMeaning = meaning.Trim();
        if (string.IsNullOrWhiteSpace(authoritativeMeaning) || authoritativeMeaning.Length > 300)
        {
            failureReason = "The authoritative meaning is empty or exceeds 300 characters.";
            return false;
        }

        var targetMeaningEntries = vocabularyEntries
            .Where(entry => string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
            .Select(entry => entry.Expression.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (direction == VietnameseToJapanese && targetMeaningEntries.Count != 1)
        {
            direction = JapaneseToVietnamese;
            prompt = $"Từ tiếng Nhật {expression} có nghĩa là gì trong tiếng Việt?";
        }

        var correctAnswerText = direction == JapaneseToVietnamese
            ? authoritativeMeaning
            : expression.Trim();

        var choicePool = direction == JapaneseToVietnamese
            ? vocabularyEntries.Select(entry => entry.Meaning)
                .Select(value => value?.Trim() ?? string.Empty)
                .Where(value => !string.IsNullOrWhiteSpace(value) && value.Length <= 300)
                .Where(value => !allCorrectMeanings.Contains(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList()
            : vocabularyEntries
                .Where(entry => !string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
                .Where(entry => !allCorrectExpressions.Contains(entry.Expression.Trim()))
                .Select(entry => entry.Expression.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value) && value.Length <= 100)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        var distinctChoicePool = choicePool
            .Where(choice => !string.Equals(choice, correctAnswerText, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var availableDistractors = distinctChoicePool
            .OrderBy(_ => Random.Shared.Next())
            .Take(3)
            .ToList();

        if (availableDistractors.Count != 3)
        {
            failureReason = $"Only {availableDistractors.Count} distinct distractors are available; three are required.";
            return false;
        }

        var choicesText = availableDistractors;
        choicesText.Add(correctAnswerText);
        for (var index = choicesText.Count - 1; index > 0; index--)
        {
            var swapIndex = Random.Shared.Next(index + 1);
            (choicesText[index], choicesText[swapIndex]) = (choicesText[swapIndex], choicesText[index]);
        }

        var correctIndexes = choicesText
            .Select((choice, index) => new { choice, index })
            .Where(item => string.Equals(item.choice, correctAnswerText, StringComparison.OrdinalIgnoreCase))
            .Select(item => item.index)
            .ToList();

        if (correctIndexes.Count != 1)
        {
            failureReason = $"Expected exactly one correct answer, found {correctIndexes.Count}.";
            return false;
        }

        var choices = choicesText
            .Select((text, index) => new QuizChoiceDto(((char)('A' + index)).ToString(), text))
            .ToList();

        result = new QuizChoiceBuildResult(
            direction,
            prompt,
            correctAnswerText,
            choices,
            choices[correctIndexes[0]].Id,
            direction == JapaneseToVietnamese
                ? JapaneseToVietnameseQuestionType
                : VietnameseToJapaneseQuestionType);
        return true;
    }
}
