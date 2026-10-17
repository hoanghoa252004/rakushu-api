using System.Text.Json;
using MediatR;
using Rakushu.Application.Usecases.Learning.Srs.Common;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttemptHandler
    : IRequestHandler<CreateQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ICurrentUserContext _currentUser;
    private readonly ISrsCardRepository _cardRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemClock _clock;

    public CreateQuizAttemptHandler(
        ICurrentUserContext currentUser,
        ISrsCardRepository cardRepository,
        IQuizAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ISystemClock clock)
    {
        _currentUser = currentUser;
        _cardRepository = cardRepository;
        _attemptRepository = attemptRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<QuizAttemptStartedResponse>> Handle(
        CreateQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteAsync(async () =>
        {
            var userId = _currentUser.UserId;
            if (userId is null)
            {
                return Result.Failure<QuizAttemptStartedResponse>(
                    UserErrors.UnauthorizedResourceAccess);
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

            // Do not silently drop selected cards: all IDs must belong to this user,
            // be active/enrolled, and (for SrsReview) already be due.
            if (cards.Count != selectedCardIds.Count)
            {
                return Result.Failure<QuizAttemptStartedResponse>(
                    QuizAttemptErrors.InvalidSelectedCards);
            }

            var cardsById = cards.ToDictionary(card => card.Id.Value);
            cards = selectedCardIds
                .Select(cardId => cardsById[cardId])
                .ToList();

            var attemptResult = QuizAttempt.Create(
                userId,
                "WORD_TO_MEANING_MCQ",
                request.AttemptMode,
                now);

            if (attemptResult.IsFailure)
            {
                return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);
            }

            var attempt = attemptResult.Value;
            var questions = new List<QuizQuestionDto>();

            for (var index = 0; index < cards.Count; index++)
            {
                var card = cards[index];
                var learningUnit = card.SrsNote.LearningUnit;
                var knowledgeUnit = learningUnit.KnowledgeMeaning.KnowledgeUnit;
                var expression = knowledgeUnit.Expression;
                var reading = knowledgeUnit.Reading;
                var correctMeaning = learningUnit.KnowledgeMeaning.Meaning;

                var optionTexts = cards
                    .Where(candidate => candidate.Id != card.Id)
                    .Select(candidate => candidate.SrsNote.LearningUnit.KnowledgeMeaning.Meaning)
                    .Where(meaning =>
                        !string.IsNullOrWhiteSpace(meaning) &&
                        !string.Equals(meaning, correctMeaning, StringComparison.Ordinal))
                    .Distinct(StringComparer.Ordinal)
                    .Take(3)
                    .Prepend(correctMeaning)
                    .ToList();

                if (optionTexts.Count < 2)
                {
                    return Result.Failure<QuizAttemptStartedResponse>(
                        QuizAttemptErrors.InvalidQuizAnswer);
                }

                // Rotate the correct answer position so it is not always the first option.
                var rotation = index % optionTexts.Count;
                optionTexts = optionTexts
                    .Skip(rotation)
                    .Concat(optionTexts.Take(rotation))
                    .ToList();

                var choices = optionTexts
                    .Select((text, choiceIndex) =>
                        new QuizChoiceDto(((char)('A' + choiceIndex)).ToString(), text))
                    .ToList();

                var correctChoice = choices.Single(choice =>
                    string.Equals(choice.Text, correctMeaning, StringComparison.Ordinal));

                var questionSnapshot = JsonSerializer.Serialize(
                    new { prompt = expression, reading },
                    JsonOptions);

                var choicesSnapshot = JsonSerializer.Serialize(choices, JsonOptions);

                var correctAnswerSnapshot = JsonSerializer.Serialize(
                    new { optionId = correctChoice.Id },
                    JsonOptions);

                var feedbackSnapshot = JsonSerializer.Serialize(
                    new
                    {
                        correct = "Chính xác.",
                        incorrect = $"Đáp án đúng: {correctMeaning}"
                    },
                    JsonOptions);

                var itemResult = QuizAttemptItem.Create(
                    card.Id,
                    index + 1,
                    "WORD_TO_MEANING_MCQ",
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

                questions.Add(new QuizQuestionDto(
                    itemResult.Value.Id.Value,
                    index + 1,
                    "WORD_TO_MEANING_MCQ",
                    expression,
                    reading,
                    choices));
            }

            _attemptRepository.Add(attempt);

            return Result.Success(new QuizAttemptStartedResponse(
                attempt.Id.Value,
                attempt.Status.ToString(),
                attempt.AttemptMode,
                attempt.StartedAt,
                questions));
        }, cancellationToken);
    }
}