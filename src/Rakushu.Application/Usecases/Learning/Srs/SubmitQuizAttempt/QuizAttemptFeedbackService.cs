using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

public interface IQuizAttemptFeedbackService
{
	Task<QuizAttemptResultResponse> ApplyAsync(
		QuizAttemptResultResponse submissionResult,
		CancellationToken cancellationToken);
}

internal sealed class QuizAttemptFeedbackService : IQuizAttemptFeedbackService
{
	private readonly ILogger<QuizAttemptFeedbackService> _logger;
	private readonly ICurrentUserContext _currentUser;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly IUnitOfWork _unitOfWork;

	public QuizAttemptFeedbackService(
		ILogger<QuizAttemptFeedbackService> logger,
		ICurrentUserContext currentUser,
		IQuizContentGenerator quizContentGenerator,
		IQuizAttemptRepository attemptRepository,
		IUnitOfWork unitOfWork)
	{
		_logger = logger;
		_currentUser = currentUser;
		_quizContentGenerator = quizContentGenerator;
		_attemptRepository = attemptRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<QuizAttemptResultResponse> ApplyAsync(
		QuizAttemptResultResponse submissionResult,
		CancellationToken cancellationToken)
	{
		var attemptId = QuizAttemptId.From(submissionResult.AttemptId);
		var attempt = await _attemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
		var userId = _currentUser.UserId;

		if (attempt is null || userId is null)
		{
			return submissionResult;
		}

		var questionMap = attempt.Quiz.Questions.ToDictionary(q => q.Id.Value);
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

		var prompts = submissionResult.Answers.Select(answer =>
		{
			questionMap.TryGetValue(answer.QuestionId, out var question);
			var savedItemId = question?.SavedItemId.Value ?? Guid.Empty;

			var history = answerHistory.TryGetValue(savedItemId, out var h)
				? h
				: new QuizItemAnswerHistory(0, 0, 0);

			var previousFeedback = feedbackHistory.TryGetValue(savedItemId, out var fh)
				? fh.Select(ToFeedbackHistoryEntry).ToArray()
				: Array.Empty<QuizFeedbackHistoryEntry>();

			var selectedChoiceText = answer.Choices?.FirstOrDefault(c =>
				string.Equals(c.Id, answer.SelectedOptionId, StringComparison.OrdinalIgnoreCase))?.Text
				?? answer.SelectedOptionId;

			var correctChoiceText = answer.Choices?.FirstOrDefault(c =>
				string.Equals(c.Id, answer.CorrectOptionId, StringComparison.OrdinalIgnoreCase))?.Text
				?? answer.CorrectOptionId;

			return new QuizFeedbackPrompt(
				answer.QuestionId,
				savedItemId,
				answer.IsCorrect,
				history.TotalAttempts,
				history.IncorrectAttempts,
				previousFeedback,
				answer.Prompt,
				answer.Reading,
				selectedChoiceText,
				correctChoiceText,
				answer.Explanation);
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

		var updatedAnswers = submissionResult.Answers
			.Select(ans => ans with
			{
				Feedback = resolvedFeedback.TryGetValue(ans.QuestionId, out var fb) ? fb : null
			})
			.ToList();

		return submissionResult with { Answers = updatedAnswers };
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
