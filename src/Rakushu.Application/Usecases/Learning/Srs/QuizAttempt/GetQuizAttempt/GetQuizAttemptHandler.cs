using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempt;

internal sealed class GetQuizAttemptHandler : IRequestHandler<GetQuizAttemptQuery, Result<QuizAttemptDetailDto>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly IUnitOfWork _unitOfWork;

	public GetQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IQuizAttemptSrsService quizAttemptSrsService,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_quizAttemptSrsService = quizAttemptSrsService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<QuizAttemptDetailDto>> Handle(
		GetQuizAttemptQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptDetailDto>(UserErrors.NotFound);

		var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(
			QuizAttemptId.From(request.AttemptId),
			cancellationToken);

		if (attempt is null)
			return Result.Failure<QuizAttemptDetailDto>(QuizAttemptErrors.NotFound);

		if (attempt.UserId != userId)
			return Result.Failure<QuizAttemptDetailDto>(QuizAttemptErrors.NotFound);

		var now = _systemClock.UtcNow;
		if (attempt.IsExpired(now))
		{
			if (!attempt.HasAnyAnsweredItems)
			{
				_quizAttemptRepository.Delete(attempt);
				await _unitOfWork.SaveChangesAsync(cancellationToken);
				return Result.Failure<QuizAttemptDetailDto>(QuizAttemptErrors.AttemptExpired);
			}

			var unanswered = attempt.ExpireAndPrune(now);
			if (unanswered.Count > 0)
			{
				_quizAttemptRepository.RemoveItems(unanswered);
			}

			await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
			_quizAttemptRepository.Update(attempt);
			await _unitOfWork.SaveChangesAsync(cancellationToken);
		}

		bool isFinished = attempt.Status == QuizAttemptStatus.Completed || attempt.Status == QuizAttemptStatus.Expired;

		var questionsDict = attempt.Quiz.Questions.ToDictionary(q => q.Id);
		var questionDtos = new List<QuizQuestionDto>(attempt.Items.Count);

		foreach (var item in attempt.Items.OrderBy(i => i.Sequence))
		{
			if (!questionsDict.TryGetValue(item.QuestionId, out var question))
				continue;

			var typeCode = question.QuestionType?.Code;
			var isTyping = string.Equals(typeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
			               string.Equals(typeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase);

			IReadOnlyList<QuizOptionDto> choices = isTyping ? [] : QuizOptionParser.Parse(question.Answers);

			if (isFinished)
			{
				bool isSkip = QuizAnswerHelper.IsSkip(item.SelectedAnswer);
				bool isCorrect = item.IsCorrect ?? false;
				int timeSpentMs = item.TimeSpentMs;

				int quality = isSkip ? 1
					: !isCorrect ? 2
					: timeSpentMs > 15_000 ? 3
					: timeSpentMs >= 5_000 ? 4
					: 5;

				questionDtos.Add(new QuizQuestionDto(
					item.Id.Value,
					item.Sequence,
					question.QuestionText,
					choices,
					item.SelectedAnswer,
					item.TimeSpentMs,
					typeCode,
					question.CorrectAnswer,
					isCorrect,
					quality,
					item.Feedback,
					question.BankQuestion?.Explanation));
			}
			else
			{
				questionDtos.Add(new QuizQuestionDto(
					item.Id.Value,
					item.Sequence,
					question.QuestionText,
					choices,
					item.SelectedAnswer,
					item.TimeSpentMs,
					typeCode,
					null,
					null,
					null,
					null,
					null));
			}
		}

		return Result.Success(new QuizAttemptDetailDto(
			attempt.Id.Value,
			attempt.Mode.ToString(),
			attempt.Status.ToString(),
			isFinished ? attempt.Score : null,
			attempt.TotalItems,
			attempt.AnsweredItems,
			isFinished ? attempt.CorrectItems : null,
			attempt.CreatedAt,
			attempt.ExpiresAt,
			attempt.CompletedAt,
			attempt.IsEvaluated,
			questionDtos));
	}
}
