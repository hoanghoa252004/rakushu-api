using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Abstractions.Persistence.Queries;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttemptHandler
	: IRequestHandler<CreateQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
	private sealed record QuizCardDetails(SrsCard Card, string Expression, string? Reading, string Meaning);

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
	private const string JapaneseToVietnamese = "JapaneseToVietnamese";
	private const string VietnameseToJapanese = "VietnameseToJapanese";
	private const string JapaneseToVietnameseQuestionType = "WORD_TO_MEANING_MCQ";
	private const string VietnameseToJapaneseQuestionType = "MEANING_TO_WORD_MCQ";

	private readonly ICurrentUserContext _currentUser;
	private readonly ISrsCardRepository _cardRepository;
	private readonly IQuizVocabularyQuery _quizVocabularyQuery;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly ILogger<CreateQuizAttemptHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ISystemClock _clock;

	public CreateQuizAttemptHandler(
		ICurrentUserContext currentUser,
		ISrsCardRepository cardRepository,
		IQuizVocabularyQuery quizVocabularyQuery,
		IQuizAttemptRepository attemptRepository,
		IQuizContentGenerator quizContentGenerator,
		ILogger<CreateQuizAttemptHandler> logger,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_cardRepository = cardRepository;
		_quizVocabularyQuery = quizVocabularyQuery;
		_attemptRepository = attemptRepository;
		_quizContentGenerator = quizContentGenerator;
		_logger = logger;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	private static bool PromptMatchesDirection(string direction, string prompt, string expression, string meaning)
	{
		return direction == JapaneseToVietnamese
			? prompt.Contains("tiếng Việt", StringComparison.OrdinalIgnoreCase) &&
				prompt.Contains(expression, StringComparison.OrdinalIgnoreCase)
			: prompt.Contains("tiếng Nhật", StringComparison.OrdinalIgnoreCase) &&
				prompt.Contains(meaning, StringComparison.OrdinalIgnoreCase) &&
				!prompt.Contains(expression, StringComparison.OrdinalIgnoreCase);
	}

	private async Task<string> ResolvePromptAsync(
		GeneratedQuizQuestion generated,
		QuizCardDetails detail,
		string questionDirection,
		string questionFormat,
		CancellationToken cancellationToken)
	{
		var promptText = generated.Prompt?.Trim() ?? string.Empty;
		var promptIsValid = promptText.Length is > 0 and <= 500 &&
			PromptMatchesDirection(questionDirection, promptText, detail.Expression, detail.Meaning);

		if (!promptIsValid)
		{
			_logger.LogWarning(
				"[QuizLLM][Retry] Invalid prompt for card {CardId}; retrying generation for this card only. PromptLength={PromptLength}; Direction={QuestionDirection}",
				detail.Card.Id.Value, promptText.Length, questionDirection);
			try
			{
				var retryQuestions = await _quizContentGenerator.GenerateQuestionsAsync(
					questionFormat,
					new[]
					{
						new QuizGenerationCard(
							detail.Card.Id.Value,
							detail.Expression,
							detail.Reading,
							detail.Meaning,
							questionDirection)
					},
					cancellationToken);
				if (retryQuestions.Count == 1 && retryQuestions[0].CardId == detail.Card.Id.Value)
				{
					var retryPrompt = retryQuestions[0].Prompt?.Trim() ?? string.Empty;
					if (retryPrompt.Length is > 0 and <= 500 &&
						PromptMatchesDirection(questionDirection, retryPrompt, detail.Expression, detail.Meaning))
						promptText = retryPrompt;
				}
			}
			catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
			{
				throw;
			}
			catch (Exception exception)
			{
				_logger.LogWarning(exception,
					"[QuizLLM][Retry] Prompt regeneration failed for card {CardId}; deterministic fallback will be used.",
					detail.Card.Id.Value);
			}

			if (promptText.Length == 0 || promptText.Length > 500 ||
				!PromptMatchesDirection(questionDirection, promptText, detail.Expression, detail.Meaning))
			{
				promptText = questionDirection == JapaneseToVietnamese
					? $"Từ tiếng Nhật {detail.Expression} có nghĩa là gì trong tiếng Việt?"
					: $"Từ tiếng Nhật nào có nghĩa là '{detail.Meaning.Trim()}'?";
				_logger.LogWarning(
					"[QuizLLM][Fallback] Using deterministic prompt for card {CardId} after targeted retry.",
					detail.Card.Id.Value);
			}
		}

		return promptText;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		CreateQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		if (!string.Equals(request.QuestionFormat, "MC", StringComparison.OrdinalIgnoreCase))
		{
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.UnsupportedQuestionFormat);
		}

		if (request.AttemptMode is not ("SrsReview" or "Practice") ||
			request.CardIds is null ||
			request.CardIds.Count is < 2 or > 50 ||
			request.CardIds.Any(cardId => cardId == Guid.Empty) ||
			request.CardIds.Distinct().Count() != request.CardIds.Count)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				QuizAttemptErrors.InvalidSelectedCards);
		}

		var now = _clock.UtcNow;
		var dueOnly = request.AttemptMode == "SrsReview";
		var selectedCardIds = request.CardIds;
		var cards = await _cardRepository.GetQuizCardsByIdsAsync(
			userId,
			selectedCardIds,
			now,
			dueOnly,
			cancellationToken);

		if (cards.Count != selectedCardIds.Count)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				QuizAttemptErrors.InvalidSelectedCards);
		}

		var cardsById = cards.ToDictionary(card => card.Id.Value);
		cards = selectedCardIds.Select(cardId => cardsById[cardId]).ToList();

		var cardDetails = cards.Select(card =>
		{
			var knowledgeUnit = card.SrsNote.LearningUnit.KnowledgeMeaning.KnowledgeUnit;
			return new QuizCardDetails(
				card,
				knowledgeUnit.Expression,
				knowledgeUnit.Reading,
				card.SrsNote.LearningUnit.KnowledgeMeaning.Meaning);
		}).ToList();

		// Every quiz is Mixed. Balance both directions, then shuffle them across the selected cards.
		var mixedDirections = Enumerable.Range(0, cardDetails.Count)
			.Select(index => index % 2 == 0 ? JapaneseToVietnamese : VietnameseToJapanese)
			.ToArray();
		Random.Shared.Shuffle(mixedDirections);
		var directionsByCardId = cardDetails
			.Select((detail, index) => new { CardId = detail.Card.Id.Value, Direction = mixedDirections[index] })
			.ToDictionary(item => item.CardId, item => item.Direction);

		IReadOnlyList<GeneratedQuizQuestion> generatedQuestions;
		try
		{
			// Call the LLM before opening the persistence transaction.
			generatedQuestions = await _quizContentGenerator.GenerateQuestionsAsync(
				request.QuestionFormat,
				cardDetails.Select(detail => new QuizGenerationCard(
					detail.Card.Id.Value,
					detail.Expression,
					detail.Reading,
					detail.Meaning,
					directionsByCardId[detail.Card.Id.Value])).ToList(),
				cancellationToken);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception exception)
		{
			_logger.LogError(exception,
				"Quiz question generation failed for {CardCount} cards using format {QuestionFormat} and mode {AttemptMode}.",
				cardDetails.Count, request.QuestionFormat, request.AttemptMode);
			return Result.Failure<QuizAttemptStartedResponse>(
				QuizAttemptErrors.QuizGenerationFailed);
		}

		var expectedCardIds = cardDetails.Select(detail => detail.Card.Id.Value).ToHashSet();
		if (generatedQuestions.Count != cardDetails.Count)
		{
			_logger.LogWarning(
				"Quiz LLM returned an unexpected question count. Expected {ExpectedCount}, actual {ActualCount}.",
				cardDetails.Count, generatedQuestions.Count);
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
		}

		if (generatedQuestions.Select(question => question.CardId).Distinct().Count() != generatedQuestions.Count)
		{
			_logger.LogWarning("Quiz LLM returned duplicate cardId values.");
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
		}

		if (!expectedCardIds.SetEquals(generatedQuestions.Select(question => question.CardId)))
		{
			_logger.LogWarning("Quiz LLM returned missing or unexpected cardId values.");
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
		}

		var questionsByCardId = generatedQuestions.ToDictionary(question => question.CardId);
		var allCorrectMeanings = cardDetails
			.Select(detail => detail.Meaning.Trim())
			.Where(meaning => !string.IsNullOrWhiteSpace(meaning))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var vocabularyEntries = await _quizVocabularyQuery.GetQuizVocabularyEntriesAsync(cancellationToken);
		var allCorrectExpressions = cardDetails.Select(detail => detail.Expression.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
		var distractorPool = vocabularyEntries.Select(entry => entry.Meaning)
			.Select(meaning => meaning?.Trim() ?? string.Empty)
			.Where(meaning => !string.IsNullOrWhiteSpace(meaning) && meaning.Length <= 300)
			.Where(meaning => !allCorrectMeanings.Contains(meaning))
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();
		var validatedQuestions = new List<(string Prompt, string? Reading, IReadOnlyList<QuizChoiceDto> Choices, string CorrectChoiceId, string QuestionType, string CorrectAnswerText)>();

		foreach (var detail in cardDetails)
		{
			var generated = questionsByCardId[detail.Card.Id.Value];
			var choicesText = new List<string>();
			var questionDirection = directionsByCardId[detail.Card.Id.Value];
			var promptText = await ResolvePromptAsync(
				generated, detail, questionDirection, request.QuestionFormat, cancellationToken);
			var authoritativeMeaning = detail.Meaning.Trim();
			if (string.IsNullOrWhiteSpace(authoritativeMeaning) || authoritativeMeaning.Length > 300)
			{
				_logger.LogWarning("Card {CardId} has an invalid authoritative meaning for quiz generation.", detail.Card.Id.Value);
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
			}

			var targetMeaningEntries = vocabularyEntries
				.Where(entry => string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
				.Select(entry => entry.Expression.Trim())
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();
			if (questionDirection == VietnameseToJapanese && targetMeaningEntries.Count != 1)
			{
				// Mixed quizzes fall back to Japanese-to-Vietnamese when the meaning is ambiguous.
				questionDirection = JapaneseToVietnamese;
				promptText = $"Từ tiếng Nhật {detail.Expression} có nghĩa là gì trong tiếng Việt?";
			}

			// Resolve the final direction; ambiguous Mixed questions fall back to JapaneseToVietnamese
			// when the Vietnamese meaning is ambiguous in the vocabulary database.
			var correctAnswerText = questionDirection == JapaneseToVietnamese
				? authoritativeMeaning
				: detail.Expression.Trim();

			var choicePool = questionDirection == JapaneseToVietnamese
				? distractorPool
				: vocabularyEntries
					.Where(entry => !string.Equals(entry.Meaning.Trim(), authoritativeMeaning, StringComparison.OrdinalIgnoreCase))
					.Where(entry => !allCorrectExpressions.Contains(entry.Expression.Trim()))
					.Select(entry => entry.Expression.Trim())
					.Where(expression => !string.IsNullOrWhiteSpace(expression) && expression.Length <= 100)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();

			var distinctChoicePool = choicePool
				.Where(choice => !string.Equals(choice, correctAnswerText, StringComparison.OrdinalIgnoreCase))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToList();
			var choiceSource = questionDirection == JapaneseToVietnamese
				? "BackendDatabase.KnowledgeMeaning.Meaning"
				: "BackendDatabase.KnowledgeUnit.Expression";
			_logger.LogInformation(
				"[QuizChoices][Source] CardId={CardId}; Direction={Direction}; Source={Source}; CandidateCount={CandidateCount}; DistinctCount={DistinctCount}; LLMGeneratedChoices=false",
				detail.Card.Id.Value, questionDirection, choiceSource, choicePool.Count, distinctChoicePool.Count);

			var availableDistractors = distinctChoicePool
				.OrderBy(_ => Random.Shared.Next())
				.Take(3)
				.ToList();
			if (availableDistractors.Count != 3)
			{
				_logger.LogWarning(
					"[QuizChoices][InsufficientDistractors] CardId={CardId}; Direction={Direction}; Source={Source}; AvailableDistinct={DistractorCount}; Required=3. No LLM retry because choices are backend-owned database data.",
					detail.Card.Id.Value, questionDirection, choiceSource, availableDistractors.Count);
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
			}

			choicesText = availableDistractors;
			choicesText.Add(correctAnswerText);
			for (var shuffleIndex = choicesText.Count - 1; shuffleIndex > 0; shuffleIndex--)
			{
				var swapIndex = Random.Shared.Next(shuffleIndex + 1);
				(choicesText[shuffleIndex], choicesText[swapIndex]) = (choicesText[swapIndex], choicesText[shuffleIndex]);
			}
			var correctChoiceIndexes = choicesText
				.Select((choice, choiceIndex) => new { choice, choiceIndex })
				.Where(choice => string.Equals(
					choice.choice,
					correctAnswerText,
					StringComparison.OrdinalIgnoreCase))
				.Select(choice => choice.choiceIndex)
				.ToList();

			if (correctChoiceIndexes.Count != 1)
			{
				_logger.LogWarning(
					"Quiz authoritative-meaning validation failed for card {CardId}. Exact matches found: {MatchCount}.",
					detail.Card.Id.Value, correctChoiceIndexes.Count);
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
			}

			_logger.LogInformation(
				"[QuizChoices][Selected] CardId={CardId}; Direction={Direction}; Source={Source}; Options={Options}; CorrectAnswerIncluded={CorrectAnswerIncluded}",
				detail.Card.Id.Value, questionDirection, choiceSource, string.Join(" | ", choicesText),
				choicesText.Count(choice => string.Equals(choice, correctAnswerText, StringComparison.OrdinalIgnoreCase)) == 1);
			var choices = choicesText
				.Select((text, choiceIndex) =>
					new QuizChoiceDto(((char)('A' + choiceIndex)).ToString(), text))
				.ToList();

			validatedQuestions.Add((
				promptText,
				questionDirection == JapaneseToVietnamese ? detail.Reading : null,
				choices,
				choices[correctChoiceIndexes[0]].Id,
				questionDirection == JapaneseToVietnamese ? JapaneseToVietnameseQuestionType : VietnameseToJapaneseQuestionType,
				correctAnswerText));
		}

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			const string attemptTemplate = "MIXED_DIRECTION_MCQ";
			var attemptResult = QuizAttempt.Create(
				userId,
				attemptTemplate,
				request.AttemptMode,
				now);

			if (attemptResult.IsFailure)
			{
				return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);
			}

			var attempt = attemptResult.Value;
			var responseQuestions = new List<QuizQuestionDto>();

			for (var index = 0; index < cardDetails.Count; index++)
			{
				var detail = cardDetails[index];
				var generated = validatedQuestions[index];
				var questionSnapshot = JsonSerializer.Serialize(new
				{
					prompt = generated.Prompt,
					reading = generated.Reading
				}, JsonOptions);
				var choicesSnapshot = JsonSerializer.Serialize(generated.Choices, JsonOptions);
				var correctAnswerSnapshot = JsonSerializer.Serialize(
					new { optionId = generated.CorrectChoiceId }, JsonOptions);
				var feedbackSnapshot = JsonSerializer.Serialize(new
				{
					correct = "Chính xác! Bạn đã chọn đúng đáp án.",
					incorrect = $"Chưa chính xác. Đáp án đúng là: {generated.CorrectAnswerText}"
				}, JsonOptions);

				var itemResult = QuizAttemptItem.Create(
					detail.Card.Id,
					index + 1,
					generated.QuestionType,
					questionSnapshot,
					correctAnswerSnapshot,
					now,
					choicesSnapshot,
					feedbackSnapshot);

				if (itemResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(itemResult.Error);
				}

				var addItemResult = attempt.AddItem(itemResult.Value);
				if (addItemResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(addItemResult.Error);
				}

				responseQuestions.Add(new QuizQuestionDto(
					itemResult.Value.Id.Value,
					index + 1,
					generated.QuestionType,
					generated.Prompt,
					generated.Reading,
					generated.Choices));
			}

			_attemptRepository.Add(attempt);
			return Result.Success(new QuizAttemptStartedResponse(
				attempt.Id.Value,
				attempt.Status.ToString(),
				attempt.AttemptMode,
				attempt.StartedAt,
				responseQuestions));
		}, cancellationToken);
	}
}
