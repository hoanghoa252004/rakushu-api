using Rakushu.Application.Abstractions.Infrastructure;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal static class QuizGenerationResultValidator
{
    public static string? Validate(
        IReadOnlyList<GeneratedQuizQuestion> generatedQuestions,
        IReadOnlyCollection<Guid> expectedCardIds)
    {
        if (generatedQuestions.Count != expectedCardIds.Count)
            return $"Unexpected question count. Expected {expectedCardIds.Count}, actual {generatedQuestions.Count}.";

        if (generatedQuestions.Select(question => question.CardId).Distinct().Count() != generatedQuestions.Count)
            return "Duplicate cardId values were returned.";

        if (!expectedCardIds.ToHashSet().SetEquals(generatedQuestions.Select(question => question.CardId)))
            return "Missing or unexpected cardId values were returned.";

        return null;
    }
}
