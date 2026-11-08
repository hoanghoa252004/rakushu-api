using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttemptAnswer;

internal sealed class SubmitQuizAttemptAnswerHandler : IRequestHandler<SubmitQuizAttemptAnswerCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitQuizAttemptAnswerHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		SubmitQuizAttemptAnswerCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptId = QuizAttemptId.From(request.AttemptId);
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
			if (attempt is null || attempt.UserId != userId)
				return Result.Failure(QuizAttemptErrors.NotFound);

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;
			if (attempt.IsExpired(now))
			{
				if (!attempt.HasAnyAnsweredItems)
				{
					_quizAttemptRepository.Delete(attempt);
				}
				else
				{
					var unanswered = attempt.ExpireAndPrune(now);
					if (unanswered.Count > 0)
					{
						_quizAttemptRepository.RemoveItems(unanswered);
					}
					_quizAttemptRepository.Update(attempt);
				}
				return Result.Failure(QuizAttemptErrors.AttemptExpired);
			}

			var item = attempt.Items.FirstOrDefault(i => i.Id.Value == request.ItemId);
			if (item is null)
				return Result.Failure(QuizAttemptItemErrors.NotFound);

			var question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == item.QuestionId);
			if (question is null)
				return Result.Failure(QuizErrors.NotFound);

			var typeCode = question.QuestionType?.Code;
			bool isSkip = QuizAnswerHelper.IsSkip(request.SelectedAnswer);
			bool isCorrect = !isSkip && CheckIsCorrect(request.SelectedAnswer, question.CorrectAnswer, question.Answers, typeCode);
			int timeSpentMs = Math.Max(0, request.TimeSpentMs);

			string storedAnswer = isSkip ? QuizAnswerHelper.Skip : request.SelectedAnswer.Trim();

			item.RecordAnswer(storedAnswer, isCorrect, timeSpentMs, null, now);
			attempt.RecalculateProgress(now);

			_quizAttemptRepository.Update(attempt);
			return Result.Success();
		}, cancellationToken);
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

		bool isTyping = string.Equals(questionTypeCode, "TYPE_READING", StringComparison.OrdinalIgnoreCase) ||
		               string.Equals(questionTypeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase) ||
		               string.Equals(questionTypeCode, "AUDIO_DICTATION", StringComparison.OrdinalIgnoreCase);

		if (!isTyping)
		{
			// MULTIPLE CHOICE:
			// 1. Direct match with correct choice id (e.g. "A" or "1")
			if (string.Equals(userAnswer, correctOptionId, StringComparison.OrdinalIgnoreCase))
				return true;

			// 2. Match with text of the correct choice
			var options = QuizOptionParser.Parse(choicesJson);
			var correctOption = options.FirstOrDefault(o => string.Equals(o.Id, correctOptionId, StringComparison.OrdinalIgnoreCase));
			if (correctOption is not null && string.Equals(userAnswer, correctOption.Text.Trim(' ', '\t', '\r', '\n', '\u3000'), StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}

		// TYPING QUESTION:
		// 1. Direct match with correct answer
		if (string.Equals(userAnswer, correctOptionId, StringComparison.OrdinalIgnoreCase))
			return true;

		// 2. Check alternative accepted answers in choicesJson (e.g. ["Cần phải ...", "Cần phải", "Cần thiết"])
		if (!string.IsNullOrWhiteSpace(choicesJson))
		{
			try
			{
				using var doc = System.Text.Json.JsonDocument.Parse(choicesJson);
				if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
				{
					foreach (var el in doc.RootElement.EnumerateArray())
					{
						string accepted = el.ValueKind == System.Text.Json.JsonValueKind.String ? el.GetString() ?? "" : "";
						if (!string.IsNullOrWhiteSpace(accepted))
						{
							if (string.Equals(userAnswer, accepted.Trim(' ', '\t', '\r', '\n', '\u3000'), StringComparison.OrdinalIgnoreCase))
								return true;

							if (string.Equals(questionTypeCode, "TYPE_MEANING", StringComparison.OrdinalIgnoreCase) && IsMeaningMatch(userAnswer, accepted))
								return true;
						}
					}
				}
			}
			catch
			{
				// Ignore json parsing issues
			}
		}

		// 3. For meaning typing, fuzzy / lenient match
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

		var correctWithoutParens = System.Text.RegularExpressions.Regex.Replace(cleanCorrect, @"\s*\([^)]*\)", "").Trim();
		if (cleanUser == correctWithoutParens)
			return true;

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
