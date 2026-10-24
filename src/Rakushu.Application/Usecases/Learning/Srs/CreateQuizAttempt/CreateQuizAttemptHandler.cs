using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
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
	private sealed record QuizItemDetails(SrsItem Item, string Expression, string? Reading, string Meaning);

	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
	private const string JapaneseToVietnamese = QuizChoiceBuilder.JapaneseToVietnamese;
	private const string VietnameseToJapanese = QuizChoiceBuilder.VietnameseToJapanese;

	// CONTEXT & CLOCK
	private readonly ICurrentUserContext _currentUser;
	private readonly ISystemClock _clock;

	// REPOSITORIES & UOW
	private readonly ISrsItemRepository _itemRepository;
	private readonly IQuizRepository _quizRepository;
	private readonly IQuizAttemptRepository _attemptRepository;
	private readonly IUnitOfWork _unitOfWork;

	// QUERIES & SERVICES
	private readonly IQuizVocabularyQuery _quizVocabularyQuery;
	private readonly ILogger<CreateQuizAttemptHandler> _logger;

	public CreateQuizAttemptHandler(
		ICurrentUserContext currentUser,
		ISrsItemRepository itemRepository,
		IQuizVocabularyQuery quizVocabularyQuery,
		IQuizRepository quizRepository,
		IQuizAttemptRepository attemptRepository,
		ILogger<CreateQuizAttemptHandler> logger,
		IUnitOfWork unitOfWork,
		ISystemClock clock)
	{
		_currentUser = currentUser;
		_itemRepository = itemRepository;
		_quizVocabularyQuery = quizVocabularyQuery;
		_quizRepository = quizRepository;
		_attemptRepository = attemptRepository;
		_logger = logger;
		_unitOfWork = unitOfWork;
		_clock = clock;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		CreateQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		// 1. Validate User Context
		var userId = _currentUser.UserId;
		if (userId is null)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				UserErrors.UnauthorizedResourceAccess);
		}

		// 2. Validate Question Format & Input Items
		if (!string.Equals(request.QuestionFormat, "MC", StringComparison.OrdinalIgnoreCase))
		{
			return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.UnsupportedQuestionFormat);
		}

		var selectedItemIds = request.ItemIds;
		if (selectedItemIds is null ||
			selectedItemIds.Count is < 2 or > 50 ||
			selectedItemIds.Any(itemId => itemId == Guid.Empty) ||
			selectedItemIds.Distinct().Count() != selectedItemIds.Count)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				QuizAttemptErrors.InvalidSelectedItems);
		}

		var now = _clock.UtcNow;
		const bool dueOnly = false;
		var items = await _itemRepository.GetQuizItemsByIdsAsync(
			userId,
			selectedItemIds,
			now,
			dueOnly,
			cancellationToken);

		if (items.Count != selectedItemIds.Count)
		{
			return Result.Failure<QuizAttemptStartedResponse>(
				QuizAttemptErrors.InvalidSelectedItems);
		}

		var itemsById = items.ToDictionary(item => item.Id.Value);
		items = selectedItemIds.Select(itemId => itemsById[itemId]).ToList();

		var itemDetails = items.Select(item =>
			new QuizItemDetails(
				item,
				item.Expression,
				item.Reading,
				item.Meaning)).ToList();

		// 3. Balance & Shuffle Question Directions (Japanese->Vietnamese vs Vietnamese->Japanese)
		var halfCount = itemDetails.Count / 2;
		var mixedDirections = Enumerable.Repeat(JapaneseToVietnamese, halfCount)
			.Concat(Enumerable.Repeat(VietnameseToJapanese, itemDetails.Count - halfCount))
			.ToArray();
		Random.Shared.Shuffle(mixedDirections);

		// 4. Fetch Vocabulary for Random Distractor Selection
		var vocabularyEntries = await _quizVocabularyQuery.GetQuizVocabularyEntriesAsync(cancellationToken);

		// 5. Build Questions & Choices Deterministically (1 correct + 3 random distractors)
		var validatedQuestions = new List<(string Prompt, string? Reading, IReadOnlyList<QuizChoiceDto> Choices, string CorrectChoiceId, string QuestionType, string CorrectAnswerText)>();

		for (var index = 0; index < itemDetails.Count; index++)
		{
			var detail = itemDetails[index];
			var direction = mixedDirections[index];

			if (!QuizChoiceBuilder.TryBuild(
				direction,
				detail.Expression,
				detail.Meaning,
				detail.Reading,
				vocabularyEntries,
				out var builtQuestion,
				out var failureReason) || builtQuestion is null)
			{
				_logger.LogWarning(
					"Quiz choice construction failed for item {ItemId}: {FailureReason}",
					detail.Item.Id.Value, failureReason);
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.QuizGenerationFailed);
			}

			_logger.LogInformation(
				"[QuizChoices][Built] ItemId={ItemId}; Direction={Direction}; Options={Options}; CorrectAnswer={CorrectAnswer}",
				detail.Item.Id.Value,
				builtQuestion.Direction,
				string.Join(" | ", builtQuestion.Choices.Select(choice => $"{choice.Id}: {choice.Text}")),
				builtQuestion.CorrectAnswerText);

			validatedQuestions.Add((
				builtQuestion.Prompt,
				builtQuestion.Direction == JapaneseToVietnamese ? detail.Reading : null,
				builtQuestion.Choices,
				builtQuestion.CorrectChoiceId,
				builtQuestion.QuestionType,
				builtQuestion.CorrectAnswerText));
		}

		// 6. Persist Quiz and Initial Attempt
		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var quizResult = Quiz.Create(userId, now);
			if (quizResult.IsFailure)
			{
				return Result.Failure<QuizAttemptStartedResponse>(quizResult.Error);
			}

			var quiz = quizResult.Value;

			for (var index = 0; index < itemDetails.Count; index++)
			{
				var detail = itemDetails[index];
				var generated = validatedQuestions[index];
				var choicesJson = JsonSerializer.Serialize(generated.Choices, JsonOptions);

				var questionResult = Question.Create(
					detail.Item.Id,
					index + 1,
					generated.QuestionType,
					generated.Prompt,
					generated.Reading,
					choicesJson,
					generated.CorrectChoiceId,
					now);

				if (questionResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(questionResult.Error);
				}

				var addQuestionResult = quiz.AddQuestion(questionResult.Value);
				if (addQuestionResult.IsFailure)
				{
					return Result.Failure<QuizAttemptStartedResponse>(addQuestionResult.Error);
				}
			}

			var attemptResult = QuizAttempt.Create(
				userId,
				quiz.Id,
				now);

			if (attemptResult.IsFailure)
			{
				return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);
			}

			var attempt = attemptResult.Value;
			var responseQuestions = new List<QuizQuestionDto>();
			var questionsList = quiz.Questions.OrderBy(q => q.Position).ToList();

			for (var index = 0; index < questionsList.Count; index++)
			{
				var question = questionsList[index];
				var detail = itemDetails[index];
				var generated = validatedQuestions[index];

				var itemResult = QuizAttemptItem.Create(
					question.Id,
					detail.Item.Id,
					index + 1,
					now);

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

			_quizRepository.Add(quiz);
			_attemptRepository.Add(attempt);

			return Result.Success(new QuizAttemptStartedResponse(
				attempt.Id.Value,
				attempt.Status.ToString(),
				attempt.Mode.ToString(),
				attempt.StartedAt,
				responseQuestions));
		}, cancellationToken);
	}
}
