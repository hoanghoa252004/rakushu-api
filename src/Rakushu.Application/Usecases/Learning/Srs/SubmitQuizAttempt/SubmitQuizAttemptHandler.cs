using System.Text.Json;
using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;

namespace Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;

internal sealed class SubmitQuizAttemptHandler : IRequestHandler<SubmitQuizAttemptCommand, Result<QuizAttemptResultResponse>>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IQuizAttemptFeedbackService _feedbackService;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IQuizAttemptFeedbackService feedbackService,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_feedbackService = feedbackService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<QuizAttemptResultResponse>> Handle(
		SubmitQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<QuizAttemptResultResponse>(UserErrors.NotFound);

		var result = await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptId = QuizAttemptId.From(request.AttemptId);
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
			if (attempt is null || attempt.UserId != userId)
				return Result.Failure<QuizAttemptResultResponse>(QuizAttemptErrors.NotFound);

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure<QuizAttemptResultResponse>(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;

			if (attempt.IsExpired(now, TimeSpan.FromHours(1)))
			{
				attempt.Abandon(now);
				return Result.Failure<QuizAttemptResultResponse>(QuizAttemptErrors.AttemptExpired);
			}

			var answersDict = request.Answers.ToDictionary(a => a.QuestionId);
			var questions = attempt.Quiz.Questions.OrderBy(q => q.Position).ToList();
			var answerResults = new List<QuizAnswerResultDto>();

			foreach (var question in questions)
			{
				answersDict.TryGetValue(question.Id.Value, out var submission);

				var typeCode = question.QuestionType?.Code;
				var isTyping = string.Equals(typeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
				               string.Equals(typeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase);

				bool isUnanswered = string.IsNullOrWhiteSpace(submission?.SelectedOptionId);
				bool isCorrect = !isUnanswered && CheckIsCorrect(
					submission!.SelectedOptionId,
					question.CorrectOptionId,
					question.ChoicesJson,
					typeCode);

				int elapsedMs = submission?.ElapsedMs ?? 0;

				// Quality scale:
				// 1: Unanswered / no submit
				// 2: Incorrect
				// 3: Correct slow (> 15s)
				// 4: Correct normal (5s - 15s)
				// 5: Correct fast (< 5s)
				int quality = isUnanswered ? 1
					: !isCorrect ? 2
					: elapsedMs > 15_000 ? 3
					: elapsedMs >= 5_000 ? 4
					: 5;

				var attemptItemResult = QuizAttemptItem.Create(
					question.Id,
					question.Position,
					now);

				if (attemptItemResult.IsFailure)
					continue;

				var attemptItem = attemptItemResult.Value;
				attemptItem.RecordAnswer(
					submission?.SelectedOptionId,
					isCorrect,
					elapsedMs,
					null,
					now);

				attempt.AddItem(attemptItem);

				// Run SM-2 only in SrsReview mode
				if (attempt.Mode == QuizAttemptMode.SrsReview)
				{
					var savedItem = await _savedItemRepository.GetByIdWithScheduleAsync(question.SavedItemId, cancellationToken);
					if (savedItem?.Schedule is not null)
					{
						var latestDetail = savedItem.Schedule.Details
							.OrderByDescending(d => d.ReviewNumber)
							.FirstOrDefault();

						int lastRepetitions = latestDetail?.Repetitions ?? 0;
						int lastInterval = latestDetail?.IntervalDays ?? 0;
						double lastFactor = latestDetail?.EasinessFactor ?? 2.5;
						int lastLapses = latestDetail?.Lapses ?? 0;
						int nextReviewNumber = (latestDetail?.ReviewNumber ?? 0) + 1;

						var sm2 = Sm2Calculator.Calculate(
							lastRepetitions,
							lastInterval,
							lastFactor,
							lastLapses,
							quality,
							now);

						savedItem.Schedule.UpdateSchedule(sm2.NewStatus, sm2.DueAt, now);

						var detailResult = SrsItemDetail.Create(
							savedItem.Schedule.Id,
							nextReviewNumber,
							quality,
							sm2.IntervalDays,
							sm2.EasinessFactor,
							sm2.Repetitions,
							sm2.Lapses,
							now,
							attemptItem.Id);

						if (detailResult.IsSuccess)
						{
							savedItem.Schedule.AddDetail(detailResult.Value);
						}
					}
				}

				IReadOnlyList<QuizOptionDto> choices = isTyping ? [] : QuizOptionParser.Parse(question.ChoicesJson);
				answerResults.Add(new QuizAnswerResultDto(
					question.Id.Value,
					question.Position,
					question.Prompt,
					question.Reading,
					choices,
					submission?.SelectedOptionId,
					question.CorrectOptionId,
					isCorrect,
					elapsedMs,
					quality,
					null,
					question.BankQuestion?.Explanation,
					typeCode));
			}

			var completeResult = attempt.Complete(now);
			if (completeResult.IsFailure)
				return Result.Failure<QuizAttemptResultResponse>(completeResult.Error);

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
		}, cancellationToken);

		if (result.IsFailure)
			return result;

		var enrichedResult = await _feedbackService.ApplyAsync(result.Value, cancellationToken);
		return Result.Success(enrichedResult);
	}

	private static bool CheckIsCorrect(
		string? userAnswer,
		string correctOptionId,
		string choicesJson,
		string? questionTypeCode)
	{
		if (string.IsNullOrWhiteSpace(userAnswer))
			return false;

		userAnswer = userAnswer.Trim(' ', '\t', '\r', '\n', '\u3000');
		correctOptionId = correctOptionId.Trim(' ', '\t', '\r', '\n', '\u3000');

		// 1. Direct case-insensitive match (e.g. multiple-choice "A" == "A" or exact text match)
		if (string.Equals(userAnswer, correctOptionId, StringComparison.OrdinalIgnoreCase))
			return true;

		// 2. Check if user answered with the option text when correctOptionId is an option key ("A", "B", etc.)
		var options = QuizOptionParser.Parse(choicesJson);
		var correctOption = options.FirstOrDefault(o => string.Equals(o.Id, correctOptionId, StringComparison.OrdinalIgnoreCase));
		if (correctOption is not null && string.Equals(userAnswer, correctOption.Text.Trim(' ', '\t', '\r', '\n', '\u3000'), StringComparison.OrdinalIgnoreCase))
			return true;

		// 3. For typing questions or general alternatives in choicesJson:
		// Check against alternative texts / IDs in choicesJson
		if (options.Any(o => string.Equals(userAnswer, o.Text.Trim(' ', '\t', '\r', '\n', '\u3000'), StringComparison.OrdinalIgnoreCase) ||
							 string.Equals(userAnswer, o.Id.Trim(' ', '\t', '\r', '\n', '\u3000'), StringComparison.OrdinalIgnoreCase)))
		{
			return true;
		}

		// 4. Special handling for TYPE_MEANING:
		// Normalize punctuation (dots, parentheses, slashes, commas, whitespace)
		if (string.Equals(questionTypeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase))
		{
			return IsMeaningMatch(userAnswer, correctOptionId);
		}

		return false;
	}

	private static bool IsMeaningMatch(string userAnswer, string correctMeaning)
	{
		static string Clean(string s)
		{
			var t = s.Trim(' ', '\t', '\r', '\n', '\u3000', '.', '…', '!', '?', '"', '\'');
			return t.ToLowerInvariant();
		}

		var cleanUser = Clean(userAnswer);
		var cleanCorrect = Clean(correctMeaning);

		if (cleanUser == cleanCorrect)
			return true;

		// Also check without parentheses, e.g. "Đã đến (khiêm nhường ngữ)" -> "Đã đến"
		var correctWithoutParens = System.Text.RegularExpressions.Regex.Replace(cleanCorrect, @"\s*\([^)]*\)", "").Trim();
		if (cleanUser == correctWithoutParens)
			return true;

		// Check synonyms split by comma or semicolon or slash
		var parts = cleanCorrect.Split(new[] { ',', ';', '/' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (var p in parts)
		{
			var cleanPart = Clean(System.Text.RegularExpressions.Regex.Replace(p, @"\s*\([^)]*\)", ""));
			if (cleanUser == cleanPart)
				return true;
		}

		return false;
	}
}
