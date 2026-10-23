using System.Text.Json;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal interface IQuizAttemptFeedbackService
{
	Task<QuizAttemptResultResponse> ApplyAsync(
		QuizAttemptResultResponse submissionResult,
		CancellationToken cancellationToken);
}

internal sealed class QuizAttemptFeedbackService : IQuizAttemptFeedbackService
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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
		var feedbackAttempt = await _attemptRepository.GetForUserAsync(
			QuizAttemptId.From(submissionResult.AttemptId),
			_currentUser.UserId!,
			cancellationToken);
		var feedbackItemsById = feedbackAttempt?.Items.ToDictionary(item => item.Id.Value);
		IReadOnlyDictionary<Guid, QuizCardAnswerHistory> answerHistoryByCardId =
			new Dictionary<Guid, QuizCardAnswerHistory>();

		if (feedbackAttempt is not null && _currentUser.UserId is { } feedbackUserId)
		{
			var cardIds = feedbackAttempt.Items.Select(item => item.SrsCardId.Value).Distinct().ToArray();
			try
			{
				answerHistoryByCardId = await _attemptRepository.GetAnswerHistoryByCardAsync(
					feedbackUserId,
					cardIds,
					QuizAttemptId.From(feedbackAttempt.Id.Value),
					cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception,
					"Could not load answer-history context for quiz attempt {AttemptId}.",
					feedbackAttempt.Id.Value);
			}
		}

		IReadOnlyDictionary<Guid, IReadOnlyList<QuizCardFeedbackHistory>> feedbackHistoryByCardId =
			new Dictionary<Guid, IReadOnlyList<QuizCardFeedbackHistory>>();
		if (feedbackAttempt is not null && _currentUser.UserId is { } historyUserId)
		{
			var cardIds = feedbackAttempt.Items.Select(item => item.SrsCardId.Value).Distinct().ToArray();
			try
			{
				feedbackHistoryByCardId = await _attemptRepository.GetFeedbackHistoryByCardAsync(
					historyUserId,
					cardIds,
					QuizAttemptId.From(feedbackAttempt.Id.Value),
					maxEntriesPerCard: 3,
					cancellationToken);
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception,
					"Could not load previous feedback context for quiz attempt {AttemptId}; feedback will use available attempt counts only.",
					feedbackAttempt.Id.Value);
			}
		}

		var feedbackPrompts = submissionResult.Results.Select(question =>
		{
			QuizAttemptItem? feedbackItem = null;
			var hasFeedbackItem = feedbackItemsById is not null &&
				feedbackItemsById.TryGetValue(question.ItemId, out feedbackItem);
			var cardId = hasFeedbackItem ? feedbackItem!.SrsCardId.Value : Guid.Empty;
			var history = answerHistoryByCardId.TryGetValue(cardId, out var cardHistory)
				? cardHistory
				: new QuizCardAnswerHistory(0, 0, 0);
			var previousFeedback = feedbackHistoryByCardId.TryGetValue(cardId, out var cardFeedback)
				? cardFeedback.Select(ToFeedbackHistoryEntry).ToArray()
				: Array.Empty<QuizFeedbackHistoryEntry>();

			var selectedChoice = question.Choices?.FirstOrDefault(c => string.Equals(c.Id, question.SelectedOptionId, StringComparison.OrdinalIgnoreCase))?.Text
				?? question.SelectedOptionId;
			var correctChoice = question.Choices?.FirstOrDefault(c => string.Equals(c.Id, question.CorrectOptionId, StringComparison.OrdinalIgnoreCase))?.Text
				?? question.CorrectOptionId;

			return new QuizFeedbackPrompt(
				question.ItemId,
				question.IsCorrect,
				history.TotalAttempts,
				history.IncorrectAttempts,
				previousFeedback,
				question.Prompt,
				question.Reading,
				selectedChoice,
				correctChoice);
		}).ToList();

		IReadOnlyDictionary<Guid, string> adaptiveFeedback;
		try
		{
			adaptiveFeedback = await _quizContentGenerator.GenerateAdaptiveFeedbackAsync(
				feedbackPrompts, cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogWarning(exception,
				"Adaptive feedback generation failed for quiz attempt {AttemptId}.",
				submissionResult.AttemptId);
			adaptiveFeedback = new Dictionary<Guid, string>();
		}

		var feedbackPromptsByItemId = feedbackPrompts.ToDictionary(prompt => prompt.ItemId);
		var updatedResults = submissionResult.Results
			.Select(question => question with
			{
				Feedback = ResolveFeedback(feedbackPromptsByItemId[question.ItemId], adaptiveFeedback)
			})
			.ToList();

		try
		{
			await _unitOfWork.ExecuteAsync(async () =>
			{
				var userId = _currentUser.UserId;
				if (userId is null)
				{
					return Result.Failure(UserErrors.UnauthorizedResourceAccess);
				}

				var attempt = await _attemptRepository.GetForUserAsync(
					QuizAttemptId.From(submissionResult.AttemptId),
					userId,
					cancellationToken);
				if (attempt is null)
				{
					return Result.Failure(QuizAttemptErrors.QuizAttemptNotFound);
				}

				foreach (var item in attempt.Items)
				{
					var feedback = ResolveFeedback(
						feedbackPromptsByItemId[item.Id.Value],
						adaptiveFeedback);
					item.UpdateFeedbackSnapshot(JsonSerializer.Serialize(
						new { adaptive = feedback }, JsonOptions));
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
			// Submission and SM-2 changes are already committed; keep returning the graded result.
			_logger.LogWarning(exception,
				"Could not persist adaptive feedback for quiz attempt {AttemptId}; returning the graded result.",
				submissionResult.AttemptId);
		}

		return submissionResult with { Results = updatedResults };
	}

	private static QuizFeedbackHistoryEntry ToFeedbackHistoryEntry(QuizCardFeedbackHistory history)
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
			return optionId ?? "Không có dữ liệu câu trả lời";

		try
		{
			using var document = JsonDocument.Parse(choicesSnapshot);
			foreach (var choice in document.RootElement.EnumerateArray())
			{
				if (TryGetStringProperty(choice, "id", out var id) &&
					string.Equals(id, optionId, StringComparison.Ordinal))
				{
					return TryGetStringProperty(choice, "text", out var text) ? text : optionId;
				}
			}
		}
		catch (JsonException)
		{
			// Historical snapshots may be malformed; retain the raw option identifier as a safe fallback.
		}

		return optionId;
	}

	private static string ResolveCorrectAnswerText(string correctAnswerSnapshot, string? choicesSnapshot)
	{
		try
		{
			using var document = JsonDocument.Parse(correctAnswerSnapshot);
			if (TryGetStringProperty(document.RootElement, "optionId", out var optionId))
				return ResolveChoiceText(choicesSnapshot, optionId);
		}
		catch (JsonException)
		{
			// Fall through to a neutral value instead of failing quiz submission.
		}

		return "Không có dữ liệu đáp án đúng";
	}

	private static string ResolvePreviousFeedback(string? feedbackSnapshot)
	{
		if (string.IsNullOrWhiteSpace(feedbackSnapshot))
			return "Không có feedback được lưu";

		try
		{
			using var document = JsonDocument.Parse(feedbackSnapshot);
			if (TryGetStringProperty(document.RootElement, "adaptive", out var feedback) &&
				!string.IsNullOrWhiteSpace(feedback))
				return feedback;
		}
		catch (JsonException)
		{
			// Fall through to a neutral value instead of failing quiz submission.
		}

		return "Không có feedback được lưu";
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

	private static string ResolveFeedback(
		QuizFeedbackPrompt prompt,
		IReadOnlyDictionary<Guid, string> adaptiveFeedback)
	{
		if (adaptiveFeedback.TryGetValue(prompt.ItemId, out var message) &&
			!string.IsNullOrWhiteSpace(message))
		{
			return message.Trim();
		}

		return string.Empty;
	}
}
