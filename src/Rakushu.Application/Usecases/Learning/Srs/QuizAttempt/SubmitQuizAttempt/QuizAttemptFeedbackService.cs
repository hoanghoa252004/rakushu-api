using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

public interface IQuizAttemptFeedbackService
{
	Task GenerateFeedbackAsync(
		QuizAttemptId attemptId,
		CancellationToken cancellationToken = default);
}

internal sealed class QuizAttemptFeedbackService : IQuizAttemptFeedbackService
{
	private readonly ILogger<QuizAttemptFeedbackService> _logger;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public QuizAttemptFeedbackService(
		ILogger<QuizAttemptFeedbackService> logger,
		IQuizContentGenerator quizContentGenerator,
		IQuizAttemptRepository attemptRepository,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_logger = logger;
		_quizContentGenerator = quizContentGenerator;
		_attemptRepository = attemptRepository;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task GenerateFeedbackAsync(
		QuizAttemptId attemptId,
		CancellationToken cancellationToken = default)
	{
		var attempt = await _attemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
		if (attempt is null)
			return;

		var userId = attempt.UserId;
		var questionMap = attempt.Quiz.Questions.ToDictionary(q => q.Id);
		var savedItemIds = attempt.Quiz.Questions.Select(q => q.SavedItemId).Distinct().ToList();

		IReadOnlyDictionary<Guid, QuizItemAnswerHistory> answerHistory = new Dictionary<Guid, QuizItemAnswerHistory>();
		try
		{
			answerHistory = await _attemptRepository.GetAnswerHistoryBySavedItemAsync(
				userId,
				savedItemIds,
				attemptId,
				cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "Could not load answer history for quiz attempt {AttemptId}", attemptId.Value);
		}

		IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>> feedbackHistory =
			new Dictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>();
		try
		{
			feedbackHistory = await _attemptRepository.GetFeedbackHistoryBySavedItemAsync(
				userId,
				savedItemIds,
				attemptId,
				maxEntriesPerItem: 3,
				cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "Could not load feedback history for quiz attempt {AttemptId}", attemptId.Value);
		}

		var answeredItems = attempt.Items.Where(i => i.AnsweredAt != null).ToList();
		if (answeredItems.Count == 0)
			return;

		var prompts = answeredItems.Select(item =>
		{
			questionMap.TryGetValue(item.QuestionId, out var question);
			var savedItemId = question?.SavedItemId.Value ?? Guid.Empty;

			var history = answerHistory.TryGetValue(savedItemId, out var h)
				? h
				: new QuizItemAnswerHistory(0, 0, 0);

			var previousFeedback = feedbackHistory.TryGetValue(savedItemId, out var fh)
				? fh.Select(ToFeedbackHistoryEntry).ToArray()
				: Array.Empty<QuizFeedbackHistoryEntry>();

			var choices = question is not null ? QuizOptionParser.Parse(question.Answers) : [];

			var selectedChoiceText = choices.FirstOrDefault(c =>
				string.Equals(c.Id, item.SelectedAnswer, StringComparison.OrdinalIgnoreCase))?.Text
				?? item.SelectedAnswer;

			var correctChoiceText = choices.FirstOrDefault(c =>
				string.Equals(c.Id, question?.CorrectAnswer, StringComparison.OrdinalIgnoreCase))?.Text
				?? question?.CorrectAnswer;

			return new QuizFeedbackPrompt(
				item.QuestionId.Value,
				savedItemId,
				item.IsCorrect ?? false,
				history.TotalAttempts,
				history.IncorrectAttempts,
				previousFeedback,
				question?.QuestionText ?? string.Empty,
				null,
				selectedChoiceText,
				correctChoiceText,
				question?.BankQuestion?.Explanation);
		}).ToList();

		IReadOnlyDictionary<Guid, string> adaptiveFeedback = new Dictionary<Guid, string>();
		try
		{
			adaptiveFeedback = await _quizContentGenerator.GenerateAdaptiveFeedbackAsync(prompts, cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "LLM adaptive feedback generation failed for attempt {AttemptId}. Using fallbacks.", attemptId.Value);
		}

		var resolvedFeedback = new Dictionary<Guid, string>();
		foreach (var prompt in prompts)
		{
			if (adaptiveFeedback.TryGetValue(prompt.QuestionId, out var msg) && !string.IsNullOrWhiteSpace(msg))
			{
				resolvedFeedback[prompt.QuestionId] = msg.Trim();
			}
			else
			{
				resolvedFeedback[prompt.QuestionId] = ResolveFallbackFeedback(prompt);
			}
		}

		try
		{
			await _unitOfWork.ExecuteAsync(async () =>
			{
				var freshAttempt = await _attemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
				if (freshAttempt is null)
					return Result.Failure(QuizAttemptErrors.NotFound);

				foreach (var item in freshAttempt.Items)
				{
					if (resolvedFeedback.TryGetValue(item.QuestionId.Value, out var fb))
					{
						item.UpdateFeedback(fb);
					}
				}

				freshAttempt.MarkEvaluated(_systemClock.UtcNow);
				_attemptRepository.Update(freshAttempt);

				return Result.Success();
			}, cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception, "Could not persist feedback for attempt {AttemptId}", attemptId.Value);
		}
	}

	private static QuizFeedbackHistoryEntry ToFeedbackHistoryEntry(QuizItemFeedbackHistory history)
	{
		var selectedAnswer = ResolveChoiceText(history.ChoicesSnapshot, history.SelectedAnswer);
		var correctAnswer = ResolveCorrectAnswerText(history.CorrectAnswerSnapshot, history.ChoicesSnapshot);
		var feedback = ResolvePreviousFeedback(history.FeedbackSnapshot);

		return new QuizFeedbackHistoryEntry(
			history.IsCorrect,
			selectedAnswer,
			correctAnswer,
			feedback,
			history.AnsweredAt);
	}

	private static string ResolveChoiceText(string? choicesSnapshot, string? optionId)
	{
		if (string.IsNullOrWhiteSpace(optionId) || string.IsNullOrWhiteSpace(choicesSnapshot))
			return optionId ?? string.Empty;

		try
		{
			using var document = JsonDocument.Parse(choicesSnapshot);
			if (document.RootElement.ValueKind == JsonValueKind.Array)
			{
				foreach (var choice in document.RootElement.EnumerateArray())
				{
					if (TryGetStringProperty(choice, "id", out var id) &&
						string.Equals(id, optionId, StringComparison.OrdinalIgnoreCase))
					{
						return TryGetStringProperty(choice, "text", out var text) ? text : optionId;
					}
				}
			}
		}
		catch (JsonException) { }

		return optionId;
	}

	private static string ResolveCorrectAnswerText(string? correctAnswerSnapshot, string? choicesSnapshot)
	{
		if (string.IsNullOrWhiteSpace(correctAnswerSnapshot))
			return "Không có dữ liệu đáp án đúng";

		var optionId = correctAnswerSnapshot;
		if (correctAnswerSnapshot.TrimStart().StartsWith("{"))
		{
			try
			{
				using var document = JsonDocument.Parse(correctAnswerSnapshot);
				if (TryGetStringProperty(document.RootElement, "optionId", out var parsedId))
					optionId = parsedId;
			}
			catch (JsonException) { }
		}

		return ResolveChoiceText(choicesSnapshot, optionId);
	}

	private static string ResolvePreviousFeedback(string? feedbackSnapshot)
	{
		if (string.IsNullOrWhiteSpace(feedbackSnapshot))
			return "Không có feedback được lưu";

		if (feedbackSnapshot.TrimStart().StartsWith("{"))
		{
			try
			{
				using var document = JsonDocument.Parse(feedbackSnapshot);
				if (TryGetStringProperty(document.RootElement, "adaptive", out var feedback) && !string.IsNullOrWhiteSpace(feedback))
					return feedback;
				if (TryGetStringProperty(document.RootElement, "message", out var msg) && !string.IsNullOrWhiteSpace(msg))
					return msg;
			}
			catch (JsonException) { }
		}

		return feedbackSnapshot;
	}

	private static bool TryGetStringProperty(JsonElement element, string propertyName, out string value)
	{
		if (element.ValueKind == JsonValueKind.Object &&
			element.TryGetProperty(propertyName, out var property) &&
			property.ValueKind == JsonValueKind.String)
		{
			value = property.GetString() ?? string.Empty;
			return true;
		}

		value = string.Empty;
		return false;
	}

	private static string ResolveFallbackFeedback(QuizFeedbackPrompt prompt)
	{
		if (prompt.IsCorrect)
		{
			return prompt.LearningContext switch
			{
				"CorrectAfterPriorMistakes" => "Xuất sắc! Lần trước bạn đã từng trả lời sai từ này, nhưng lần này bạn đã ghi nhớ rất chính xác!",
				"CorrectAgain" => "Rất tốt! Bạn tiếp tục duy trì phong độ ghi nhớ chính xác từ này.",
				"FirstCorrect" => "Chính xác! Khởi đầu rất tốt với từ vựng này.",
				_ => "Chính xác! Bạn đã ghi nhớ đúng từ vựng này."
			};
		}

		return prompt.LearningContext switch
		{
			"IncorrectAfterPriorCorrect" => !string.IsNullOrWhiteSpace(prompt.CorrectAnswer)
				? $"Tiếc quá! Lần trước bạn đã nhớ đúng từ này rồi, hãy ôn lại để không bị quên nhé! Đáp án đúng là \"{prompt.CorrectAnswer}\"."
				: "Tiếc quá! Lần trước bạn đã nhớ đúng từ này rồi, hãy ôn tập lại để không bị quên nhé!",
			"RepeatedIncorrect" => !string.IsNullOrWhiteSpace(prompt.CorrectAnswer)
				? $"Từ này có vẻ khó nhớ, bạn hãy ghi chú lại và xem kỹ giải thích để nhớ lâu hơn nhé! Đáp án đúng là \"{prompt.CorrectAnswer}\"."
				: "Từ này có vẻ khó nhớ, bạn hãy ghi chú lại và xem kỹ giải thích để nhớ lâu hơn nhé!",
			"FirstIncorrect" => !string.IsNullOrWhiteSpace(prompt.CorrectAnswer)
				? $"Chưa chính xác. Đừng nản lòng, hãy xem kỹ giải thích để ghi nhớ từ này nhé! Đáp án đúng là \"{prompt.CorrectAnswer}\"."
				: "Chưa chính xác. Đừng nản lòng, hãy xem kỹ giải thích để ghi nhớ từ này nhé!",
			_ => !string.IsNullOrWhiteSpace(prompt.CorrectAnswer)
				? $"Chưa chính xác. Đáp án đúng là \"{prompt.CorrectAnswer}\". Hãy chú ý ôn tập lại từ này nhé!"
				: "Chưa chính xác. Hãy chú ý ôn tập lại từ này nhé!"
		};
	}
}
