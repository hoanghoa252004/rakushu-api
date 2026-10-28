using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.GetQuizAttemptResult;

internal sealed class GetQuizAttemptResultHandler : IRequestHandler<GetQuizAttemptResultQuery, Result<QuizAttemptResultResponse>>
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

	public async Task<Result<QuizAttemptResultResponse>> Handle(
		GetQuizAttemptResultQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptResultResponse>(UserErrors.NotFound);

		var attemptId = QuizAttemptId.From(request.AttemptId);
		var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
		if (attempt is null || attempt.UserId != userId)
			return Result.Failure<QuizAttemptResultResponse>(QuizAttemptErrors.NotFound);

		var itemsDict = attempt.Items.ToDictionary(i => i.QuestionId);
		var answerResults = new List<QuizAnswerResultDto>();

		foreach (var question in attempt.Quiz.Questions.OrderBy(q => q.Position))
		{
			itemsDict.TryGetValue(question.Id, out var attemptItem);

			bool isUnanswered = string.IsNullOrWhiteSpace(attemptItem?.SelectedOptionId);
			bool isCorrect = attemptItem?.IsCorrect ?? false;
			int elapsedMs = attemptItem?.ElapsedMs ?? 0;

			int quality = isUnanswered ? 1
				: !isCorrect ? 2
				: elapsedMs > 15_000 ? 3
				: elapsedMs >= 5_000 ? 4
				: 5;

			var choices = QuizOptionParser.Parse(question.ChoicesJson);
			answerResults.Add(new QuizAnswerResultDto(
				question.Id.Value,
				question.Position,
				question.Prompt,
				question.Reading,
				choices,
				attemptItem?.SelectedOptionId,
				question.CorrectOptionId,
				isCorrect,
				elapsedMs,
				quality,
				attemptItem?.Feedback,
				question.BankQuestion?.Explanation));
		}

		return Result.Success(new QuizAttemptResultResponse(
			attempt.Id.Value,
			attempt.QuizId.Value,
			attempt.Mode.ToString(),
			attempt.Status.ToString(),
			attempt.Score ?? 0,
			attempt.TotalItems,
			attempt.AnsweredItems,
			attempt.CorrectItems,
			attempt.StartedAt,
			attempt.CompletedAt,
			answerResults));
	}
}
