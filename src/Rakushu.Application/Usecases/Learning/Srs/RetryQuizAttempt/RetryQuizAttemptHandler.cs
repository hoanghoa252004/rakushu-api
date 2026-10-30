using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.Quiz.Question;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.RetryQuizAttempt;

internal sealed class RetryQuizAttemptHandler : IRequestHandler<RetryQuizAttemptCommand, Result<QuizAttemptStartedResponse>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly IQuizRepository _quizRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public RetryQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		IQuizRepository quizRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_quizRepository = quizRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<QuizAttemptStartedResponse>> Handle(
		RetryQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptStartedResponse>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var sourceAttemptId = QuizAttemptId.From(request.AttemptId);
			var sourceAttempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(sourceAttemptId, cancellationToken);
			if (sourceAttempt is null || sourceAttempt.UserId != userId)
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.NotFound);

			if (sourceAttempt.Status != QuizAttemptStatus.Completed)
				return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.NotCompleted);

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

			bool mistakesOnly = string.Equals(request.RetryMode, "MistakesOnly", StringComparison.OrdinalIgnoreCase);

			Quiz quizToUse;
			if (mistakesOnly)
			{
				var wrongQuestionIds = sourceAttempt.Items
					.Where(item => item.IsCorrect == false)
					.Select(item => item.QuestionId)
					.ToHashSet();

				var wrongQuestions = sourceAttempt.Quiz.Questions
					.Where(q => wrongQuestionIds.Contains(q.Id))
					.OrderBy(q => q.Position)
					.ToList();

				if (wrongQuestions.Count == 0)
					return Result.Failure<QuizAttemptStartedResponse>(QuizAttemptErrors.NoMistakesToRetry);

				var newQuizResult = Quiz.Create(userId, now);
				if (newQuizResult.IsFailure)
					return Result.Failure<QuizAttemptStartedResponse>(newQuizResult.Error);

				var newQuiz = newQuizResult.Value;
				int pos = 1;
				foreach (var q in wrongQuestions)
				{
					var newQuestionResult = Question.Create(
						q.SavedItemId,
						q.QuestionTypeId,
						pos++,
						q.Prompt,
						q.Reading,
						q.ChoicesJson,
						q.CorrectOptionId,
						now,
						q.BankQuestionId);

					if (newQuestionResult.IsSuccess)
					{
						newQuiz.AddQuestion(newQuestionResult.Value);
					}
				}

				_quizRepository.Add(newQuiz);
				quizToUse = newQuiz;
			}
			else
			{
				quizToUse = sourceAttempt.Quiz;
			}

			var attemptMode = mistakesOnly ? QuizAttemptMode.PracticeMistakes : QuizAttemptMode.PracticeAll;

			var newAttemptResult = QuizAttempt.Create(
				userId,
				quizToUse.Id,
				now,
				attemptMode);

			if (newAttemptResult.IsFailure)
				return Result.Failure<QuizAttemptStartedResponse>(newAttemptResult.Error);

			var newAttempt = newAttemptResult.Value;
			_quizAttemptRepository.Add(newAttempt);

			var clientQuestions = quizToUse.Questions
				.OrderBy(q => q.Position)
				.Select(q =>
				{
					var typeCode = q.QuestionType?.Code;
					var isTyping = string.Equals(typeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
					               string.Equals(typeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase);
					IReadOnlyList<QuizOptionDto> choices = isTyping ? [] : QuizOptionParser.Parse(q.ChoicesJson);
					return new QuizQuestionDto(
						q.Id.Value,
						q.Position,
						q.Prompt,
						q.Reading,
						choices,
						typeCode);
				})
				.ToList();

			return Result.Success(new QuizAttemptStartedResponse(
				newAttempt.Id.Value,
				quizToUse.Id.Value,
				newAttempt.Mode.ToString(),
				quizToUse.TotalQuestions,
				newAttempt.StartedAt,
				newAttempt.StartedAt.AddHours(1),
				clientQuestions));
		}, cancellationToken);
	}
}
