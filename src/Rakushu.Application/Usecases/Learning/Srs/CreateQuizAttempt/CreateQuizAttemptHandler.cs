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
		var validationError = QuizGenerationResultValidator.Validate(generatedQuestions, expectedCardIds);
		if (validationError is not null)
		{
			_logger.LogWarning("Quiz LLM result validation failed: {ValidationError}", validationError);
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
		}

		var questionsByCardId = generatedQuestions.ToDictionary(question => question.CardId);
		var allCorrectMeanings = cardDetails
			.Select(detail => detail.Meaning.Trim())
			.Where(meaning => !string.IsNullOrWhiteSpace(meaning))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var vocabularyEntries = await _quizVocabularyQuery.GetQuizVocabularyEntriesAsync(cancellationToken);
		var allCorrectExpressions = cardDetails.Select(detail => detail.Expression.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);

		var validatedQuestions = new List<(string Prompt, string? Reading, IReadOnlyList<QuizChoiceDto> Choices, string CorrectChoiceId, string QuestionType, string CorrectAnswerText)>();

		foreach (var detail in cardDetails)
		{
			var generated = questionsByCardId[detail.Card.Id.Value];
			var questionDirection = directionsByCardId[detail.Card.Id.Value];
			var promptText = await QuizPromptResolver.ResolveAsync(
				_quizContentGenerator,
				_logger,
				detail.Card.Id.Value,
				detail.Expression,
				detail.Reading,
				detail.Meaning,
				questionDirection,
				request.QuestionFormat,
				generated,
				cancellationToken);

			if (!QuizChoiceBuilder.TryBuild(
				questionDirection,
				detail.Expression,
				detail.Meaning,
				detail.Reading,
				vocabularyEntries,
				allCorrectMeanings,
				allCorrectExpressions,
				promptText,
				out var builtQuestion,
				out var failureReason) || builtQuestion is null)
			{
				_logger.LogWarning(
					"Quiz choice construction failed for card {CardId}: {FailureReason}",
					detail.Card.Id.Value, failureReason);
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
			}

			_logger.LogInformation(
				"[QuizChoices][Selected] CardId={CardId}; Direction={Direction}; Options={Options}; CorrectAnswerIncluded={CorrectAnswerIncluded}",
				detail.Card.Id.Value,
				builtQuestion.Direction,
				string.Join(" | ", builtQuestion.Choices.Select(choice => choice.Text)),
				builtQuestion.Choices.Count(choice => string.Equals(
					choice.Text, builtQuestion.CorrectAnswerText, StringComparison.OrdinalIgnoreCase)) == 1);

			validatedQuestions.Add((
				builtQuestion.Prompt,
				builtQuestion.Direction == JapaneseToVietnamese ? detail.Reading : null,
				builtQuestion.Choices,
				builtQuestion.CorrectChoiceId,
				builtQuestion.QuestionType,
				builtQuestion.CorrectAnswerText));
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
