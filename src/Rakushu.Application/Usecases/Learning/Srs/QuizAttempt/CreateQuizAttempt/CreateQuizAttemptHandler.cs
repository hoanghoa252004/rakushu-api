using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.AiService;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs.BankQuestion;
using Rakushu.Domain.Entities.Srs.QuestionType;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

using BankQuestionEntity = Rakushu.Domain.Entities.Srs.BankQuestion.BankQuestion;
using QuizEntity = Rakushu.Domain.Entities.Srs.Quiz.Quiz;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.CreateQuizAttempt;

internal sealed class CreateQuizAttemptHandler : IRequestHandler<CreateQuizAttemptCommand, Result<Guid>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly IBankQuestionRepository _bankQuestionRepository;
	private readonly ILearningUnitRepository _learningUnitRepository;
	private readonly IQuizRepository _quizRepository;
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizContentGenerator _quizContentGenerator;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public CreateQuizAttemptHandler(
		ISavedItemRepository savedItemRepository,
		IBankQuestionRepository bankQuestionRepository,
		ILearningUnitRepository learningUnitRepository,
		IQuizRepository quizRepository,
		IQuizAttemptRepository quizAttemptRepository,
		IQuizContentGenerator quizContentGenerator,
		IQuizAttemptSrsService quizAttemptSrsService,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_savedItemRepository = savedItemRepository;
		_bankQuestionRepository = bankQuestionRepository;
		_learningUnitRepository = learningUnitRepository;
		_quizRepository = quizRepository;
		_quizAttemptRepository = quizAttemptRepository;
		_quizContentGenerator = quizContentGenerator;
		_quizAttemptSrsService = quizAttemptSrsService;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<Guid>> Handle(
		CreateQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<Guid>(UserErrors.NotFound);

		if (request.ItemIds is null || request.ItemIds.Count == 0)
			return Result.Failure<Guid>(QuizErrors.EmptySelectedItems);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var activeAttempt = await _quizAttemptRepository.GetActiveAttemptForUserAsync(userId, cancellationToken);
			if (activeAttempt is not null)
			{
				if (activeAttempt.IsExpired(now))
				{
					activeAttempt.Expire(now);
					if (activeAttempt.HasAnyAnsweredItems)
					{
						await _quizAttemptSrsService.ApplySrsUpdatesAsync(activeAttempt, now, cancellationToken);
					}
					_quizAttemptRepository.Update(activeAttempt);
				}
				else if (request.DiscardActive)
				{
					var cancelResult = activeAttempt.Cancel(now);
					if (cancelResult.IsFailure)
						return Result.Failure<Guid>(cancelResult.Error);

					_quizAttemptRepository.Update(activeAttempt);
				}
				else
				{
					return Result.Failure<Guid>(QuizAttemptErrors.ActiveAttemptInProgressWithId(activeAttempt.Id.Value));
				}
			}

			var ids = request.ItemIds.Select(SavedItemId.From).Distinct().ToList();
			var items = await _savedItemRepository.GetByIdsWithScheduleAsync(ids, userId, cancellationToken);

			if (items.Count == 0)
				return Result.Failure<Guid>(QuizErrors.NoQuestionsAvailable);

			var meaningIds = items.Select(i => i.KnowledgeMeaningId).Distinct().ToList();
			var bankQuestions = (await _bankQuestionRepository.GetByMeaningIdsAsync(meaningIds, cancellationToken)).ToList();

			var quizResult = QuizEntity.Create(userId, now);
			if (quizResult.IsFailure)
				return Result.Failure<Guid>(quizResult.Error);

			var quiz = quizResult.Value;
			int position = 1;

			foreach (var item in items)
			{
				var matchingQuestions = bankQuestions
					.Where(bq => bq.KnowledgeMeaningId == item.KnowledgeMeaningId)
					.ToList();

				if (matchingQuestions.Count == 0)
					continue;

				// Pick question adaptively based on item's SRS repetition level
				var bankQ = SrsQuestionSelector.PickQuestionForSrsStage(item, matchingQuestions);

				var questionResult = Question.Create(
					item.Id,
					bankQ.Id,
					position,
					bankQ.Question,
					bankQ.Answers,
					bankQ.CorrectAnswer,
					bankQ.MediaType,
					bankQ.MediaUrl,
					bankQ.MediaMetadata);

				if (questionResult.IsFailure)
					continue;

				quiz.AddQuestion(questionResult.Value);
				position++;
			}

			if (quiz.TotalQuestions == 0)
				return Result.Failure<Guid>(QuizErrors.NoQuestionsAvailable);

			var attemptMode = string.Equals(request.Mode, "Cramming", StringComparison.OrdinalIgnoreCase)
				? QuizAttemptMode.PracticeAll
				: QuizAttemptMode.SrsReview;

			var attemptResult = Domain.Entities.Srs.QuizAttempt.QuizAttempt.Create(
				userId, quiz.Id, now, quiz.TotalQuestions, attemptMode);
			if (attemptResult.IsFailure)
				return Result.Failure<Guid>(attemptResult.Error);

			var attempt = attemptResult.Value;

			_quizRepository.Add(quiz);
			_quizAttemptRepository.Add(attempt);

			return Result.Success(attempt.Id.Value);
		}, cancellationToken);
	}
}
