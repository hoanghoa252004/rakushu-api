using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

/// <summary>Validates generated prompts, retries invalid prompts once, and supplies deterministic fallback text.</summary>
internal static class QuizPromptResolver
{
	private const string JapaneseToVietnamese = "JapaneseToVietnamese";

	public static async Task<string> ResolveAsync(
		IQuizContentGenerator generator,
		ILogger logger,
		Guid cardId,
		string expression,
		string? reading,
		string meaning,
		string questionDirection,
		string questionFormat,
		GeneratedQuizQuestion generated,
		CancellationToken cancellationToken)
	{
		var promptText = generated.Prompt?.Trim() ?? string.Empty;
		if (IsValidPrompt(questionDirection, promptText, expression, meaning))
			return promptText;

		logger.LogWarning(
			"[QuizLLM][Retry] Invalid prompt for card {CardId}; retrying generation for this card only. PromptLength={PromptLength}; Direction={QuestionDirection}",
			cardId, promptText.Length, questionDirection);

		try
		{
			var retryQuestions = await generator.GenerateQuestionsAsync(
				questionFormat,
				new[] { new QuizGenerationCard(cardId, expression, reading, meaning, questionDirection) },
				cancellationToken);

			if (retryQuestions.Count == 1 && retryQuestions[0].CardId == cardId)
			{
				var retryPrompt = retryQuestions[0].Prompt?.Trim() ?? string.Empty;
				if (IsValidPrompt(questionDirection, retryPrompt, expression, meaning))
					promptText = retryPrompt;
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			logger.LogWarning(exception,
				"[QuizLLM][Retry] Prompt regeneration failed for card {CardId}; deterministic fallback will be used.",
				cardId);
		}

		if (!IsValidPrompt(questionDirection, promptText, expression, meaning))
		{
			promptText = questionDirection == JapaneseToVietnamese
				? $"Từ tiếng Nhật {expression} có nghĩa là gì trong tiếng Việt?"
				: $"Từ tiếng Nhật nào có nghĩa là '{meaning.Trim()}'?";

			logger.LogWarning(
				"[QuizLLM][Fallback] Using deterministic prompt for card {CardId} after targeted retry.",
				cardId);
		}

		return promptText;
	}

	private static bool IsValidPrompt(string direction, string prompt, string expression, string meaning)
	{
		if (prompt.Length is 0 or > 500)
			return false;

		return direction == JapaneseToVietnamese
			? prompt.Contains("tiếng Việt", StringComparison.OrdinalIgnoreCase)
				&& prompt.Contains(expression, StringComparison.OrdinalIgnoreCase)
			: prompt.Contains("tiếng Nhật", StringComparison.OrdinalIgnoreCase)
				&& prompt.Contains(meaning, StringComparison.OrdinalIgnoreCase)
				&& !prompt.Contains(expression, StringComparison.OrdinalIgnoreCase);
	}
}
