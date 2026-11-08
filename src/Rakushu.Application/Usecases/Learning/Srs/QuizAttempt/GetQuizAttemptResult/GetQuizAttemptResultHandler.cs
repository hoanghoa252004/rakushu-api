using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResultHandler : IRequestHandler<GetQuizAttemptResultQuery, Result<QuizAttemptResultDto>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;

	public GetQuizAttemptResultHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
	}

	public async Task<Result<QuizAttemptResultDto>> Handle(
		GetQuizAttemptResultQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptResultDto>(UserErrors.NotFound);

		var attemptId = QuizAttemptId.From(request.AttemptId);
		var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
		if (attempt is null || attempt.UserId != userId)
			return Result.Failure<QuizAttemptResultDto>(QuizAttemptErrors.NotFound);

		if (attempt.Status != QuizAttemptStatus.Completed && attempt.Status != QuizAttemptStatus.Expired)
			return Result.Failure<QuizAttemptResultDto>(QuizAttemptErrors.NotCompleted);

		var questionsDict = attempt.Quiz.Questions.ToDictionary(q => q.Id);
		var answerResults = new List<QuizAnswerResultDto>(attempt.Items.Count);

		foreach (var item in attempt.Items.OrderBy(i => i.Sequence))
		{
			if (!questionsDict.TryGetValue(item.QuestionId, out var question))
				continue;

			bool isSkip = QuizAnswerHelper.IsSkip(item.SelectedAnswer);
			bool isCorrect = item.IsCorrect ?? false;
			int timeSpentMs = item.TimeSpentMs;

			int quality = isSkip ? 1
				: !isCorrect ? 2
				: timeSpentMs > 15_000 ? 3
				: timeSpentMs >= 5_000 ? 4
				: 5;

			var typeCode = question.QuestionType?.Code;
			var isTyping = string.Equals(typeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
			               string.Equals(typeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase);

			IReadOnlyList<QuizOptionDto> choices = isTyping ? [] : QuizOptionParser.Parse(question.Answers);
			answerResults.Add(new QuizAnswerResultDto(
				item.Id.Value,
				item.Sequence,
				question.QuestionText,
				choices,
				item.SelectedAnswer,
				question.CorrectAnswer,
				isCorrect,
				timeSpentMs,
				quality,
				item.Feedback,
				question.BankQuestion?.Explanation,
				typeCode));
		}

		return Result.Success(new QuizAttemptResultDto(
			attempt.Id.Value,
			attempt.Mode.ToString(),
			attempt.Status.ToString(),
			attempt.Score ?? 0,
			attempt.TotalItems,
			attempt.AnsweredItems,
			attempt.CorrectItems,
			attempt.CreatedAt,
			attempt.CompletedAt,
			attempt.IsEvaluated,
			answerResults));
	}
}
