using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.CreateQuizAttempt;

internal sealed class CreateQuizAttemptHandler : IRequestHandler<CreateQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly IQuizRepository _quizRepository;
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateQuizAttemptHandler(
		ISavedItemRepository savedItemRepository,
		IBankQuestionRepository bankQuestionRepository,
		IQuizRepository quizRepository,
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_quizRepository = quizRepository;
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		CreateQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptStartedResponse>(UserErrors.NotFound);

		if (request.ItemIds is null || request.ItemIds.Count == 0)
			return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.EmptySelectedItems);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var activeAttempt = await _quizAttemptRepository.GetActiveAttemptForUserAsync(userId, cancellationToken);
			if (activeAttempt is not null)
			{
				if (activeAttempt.IsExpired(now, TimeSpan.FromHours(1)))
				{
					activeAttempt.Abandon(now);
				}
				else
				{
					return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.ActiveAttemptInProgress);
				}
			}

			var ids = request.ItemIds.Select(SavedItemId.From).Distinct().ToList();
			var items = await _savedItemRepository.GetByIdsWithScheduleAsync(ids, userId, cancellationToken);

			if (items.Count == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var meaningIds = items.Select(i => i.KnowledgeMeaningId).Distinct().ToList();
			var bankQuestions = await _bankQuestionRepository.GetByMeaningIdsAsync(meaningIds, cancellationToken);

			if (bankQuestions.Count == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var quizResult = Quiz.Create(userId, now);
			if (quizResult.IsFailure)
				return Result.Failure<QuizAttemptStartedResponse>(quizResult.Error);

			var quiz = quizResult.Value;
			var clientQuestions = new List<QuizQuestionDto>();
			int position = 1;

			foreach (var item in items)
			{
				var matchingQuestions = bankQuestions
					.Where(bq => bq.KnowledgeMeaningId == item.KnowledgeMeaningId)
					.ToList();

				if (matchingQuestions.Count == 0)
					continue;

				// Pick first or random matching question
				var bankQ = matchingQuestions[Random.Shared.Next(matchingQuestions.Count)];

				var questionResult = Question.Create(
					item.Id,
					bankQ.QuestionTypeId,
					position,
					bankQ.Prompt,
					item.Reading,
					bankQ.ChoicesJson,
					bankQ.CorrectChoiceId,
					now,
					bankQ.Id);

				if (questionResult.IsFailure)
					continue;

				var question = questionResult.Value;
				quiz.AddQuestion(question);

				var choices = QuizOptionParser.Parse(question.ChoicesJson);
				clientQuestions.Add(new QuizQuestionDto(
					question.Id.Value,
					position,
					question.Prompt,
					question.Reading,
					bankQ.AudioUrl,
					choices));

				position++;
			}

			if (quiz.TotalQuestions == 0)
				return Result.Failure<QuizAttemptStartedResponse>(QuizErrors.NoQuestionsAvailable);

			var attemptResult = QuizAttempt.Create(userId, quiz.Id, now, QuizAttemptMode.SrsReview);
			if (attemptResult.IsFailure)
				return Result.Failure<QuizAttemptStartedResponse>(attemptResult.Error);

			var attempt = attemptResult.Value;

			_quizRepository.Add(quiz);
			_quizAttemptRepository.Add(attempt);

			return Result.Success(new QuizAttemptStartedResponse(
				attempt.Id.Value,
				quiz.Id.Value,
				attempt.Mode.ToString(),
				quiz.TotalQuestions,
				attempt.StartedAt,
				attempt.StartedAt.AddHours(1),
				clientQuestions));
		}, cancellationToken);
	}
}
